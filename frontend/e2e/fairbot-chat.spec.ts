import { test, expect, type Page } from '@playwright/test'

async function mockAuthenticatedAdmin(page: Page) {
  const accessToken = 'e2e-access-token'

  await page.route('**/api/auth/refresh', async route => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({
        code: 200,
        msg: 'Refresh successful',
        data: { accessToken },
      }),
    })
  })
}

const asFairBotEnvelope = (message: string, note?: string | null) => ({
  code: 200,
  msg: 'Chat response received',
  data: {
    status: 'success',
    message: 'ok',
    file_name: null,
    routed_to: 'compliance_qa',
    result: {
      type: 'compliance',
      message,
      file_name: null,
      model: 'stub-model',
      note: note ?? null,
      sources: [],
    },
  },
})

function messageInput(page: Page) {
  return page.getByLabel('Message')
}

test.describe('FairBot chat critical flows', () => {
  test.beforeEach(async ({ page }) => {
    await mockAuthenticatedAdmin(page)
    await page.goto('/fairbot')
    await expect(page).toHaveURL(/\/fairbot/, { timeout: 15_000 })
    await expect(page.getByText('Welcome back')).toBeVisible({
      timeout: 15_000,
    })
  })

  test('action cards render and navigate to correct routes', async ({
    page,
  }) => {
    const rosterCard = page.getByRole('button', {
      name: /Check Roster|Roster Check/i,
    })
    const payrollCard = page.getByRole('button', {
      name: /Verify Payroll|Payroll Check/i,
    })

    await expect(rosterCard).toBeVisible()
    await expect(payrollCard).toBeVisible()

    await rosterCard.click()
    await expect(page).toHaveURL(/\/roster/)
  })

  test('general mode returns a normal answer', async ({ page }) => {
    await page.route('**/api/fairbot/chat', async route => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(
          asFairBotEnvelope('General compliance answer from FairBot.')
        ),
      })
    })

    const input = messageInput(page)
    await input.fill('What is minimum shift hours?')
    await page.getByRole('button', { name: 'Send message' }).click()

    await expect(
      page.getByText('General compliance answer from FairBot.')
    ).toBeVisible({ timeout: 10_000 })
  })

  test('ask fairbot explain path sends roster explain context and returns answer', async ({
    page,
  }) => {
    const rosterId = '11111111-1111-1111-1111-111111111111'
    const validationId = '22222222-2222-2222-2222-222222222222'
    let capturedPostData = ''

    // Register all route mocks BEFORE navigation to avoid race conditions
    await page.route('**/api/roster/*/validation', async route => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: 200,
          msg: 'Validation retrieved',
          data: {
            validationId,
            status: 'completed',
            totalShifts: 12,
            passedShifts: 9,
            failedShifts: 3,
            totalIssues: 5,
            criticalIssues: 1,
            affectedEmployees: 2,
            weekStartDate: '2026-02-23',
            weekEndDate: '2026-03-01',
            totalEmployees: 4,
            validatedAt: '2026-02-26T00:00:00Z',
            failureType: 'Compliance',
            retriable: false,
            issues: [],
          },
        }),
      })
    })

    await page.route('**/api/fairbot/chat', async route => {
      capturedPostData = route.request().postData() ?? ''
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(
          asFairBotEnvelope('Explained using roster context.')
        ),
      })
    })

    // Navigate AFTER routes are registered
    await page.goto(
      `/fairbot?intent=roster&rosterId=${rosterId}&validationId=${validationId}`
    )
    await expect(page.getByText('Roster context loaded')).toBeVisible({
      timeout: 15_000,
    })

    const input = messageInput(page)
    await input.fill('Please explain this roster result.')
    await page.getByRole('button', { name: 'Send message' }).click()

    await expect(
      page.getByText('Explained using roster context.')
    ).toBeVisible({ timeout: 10_000 })

    // Assert post data outside of route handler for reliability
    expect(capturedPostData).toContain('roster_explain')
    expect(capturedPostData).toContain('roster_validation')
  })

  test('explain fallback retries as general when roster context is required', async ({
    page,
  }) => {
    const rosterId = '33333333-3333-3333-3333-333333333333'
    const validationId = '44444444-4444-4444-4444-444444444444'
    let chatRequestCount = 0
    const requestBodies: string[] = []

    // Register all route mocks BEFORE navigation
    await page.route('**/api/roster/*/validation', async route => {
      await route.fulfill({
        status: 404,
        contentType: 'application/json',
        body: JSON.stringify({ code: 404, msg: 'Validation not found' }),
      })
    })

    await page.route('**/api/fairbot/chat', async route => {
      chatRequestCount += 1
      const body = route.request().postData() ?? ''
      requestBodies.push(body)

      if (chatRequestCount === 1) {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(
            asFairBotEnvelope(
              'I can explain roster results once you provide a roster validation context.',
              'ROSTER_CONTEXT_REQUIRED'
            )
          ),
        })
        return
      }

      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(
          asFairBotEnvelope('Fallback to general answer worked.')
        ),
      })
    })

    // Navigate AFTER routes are registered
    await page.goto(
      `/fairbot?intent=roster&rosterId=${rosterId}&validationId=${validationId}`
    )
    await expect(page.getByText('Roster context loaded')).toBeVisible({
      timeout: 15_000,
    })

    const input = messageInput(page)
    await input.fill('Please explain this result.')
    await page.getByRole('button', { name: 'Send message' }).click()

    await expect(
      page.getByText('Fallback to general answer worked.')
    ).toBeVisible({ timeout: 10_000 })

    // Verify ROSTER_CONTEXT_REQUIRED reply is not shown to user
    await expect(
      page.getByText(
        'I can explain roster results once you provide a roster validation context.'
      )
    ).toBeHidden()

    await expect.poll(() => chatRequestCount, { timeout: 10_000 }).toBe(2)
    // First request uses roster intent
    expect(requestBodies[0]).toContain('roster')
    // Second request retries as compliance
    expect(requestBodies[1]).toContain('compliance')
  })

  test('timeout error displays user-friendly message', async ({ page }) => {
    await page.route('**/api/fairbot/chat', async route => {
      await route.fulfill({
        status: 504,
        contentType: 'application/json',
        body: JSON.stringify({ code: 504, msg: 'Gateway timeout' }),
      })
    })

    const input = messageInput(page)
    await input.fill('What are penalty rates?')
    await page.getByRole('button', { name: 'Send message' }).click()

    await expect(page.getByText(/timed out/i)).toBeVisible({ timeout: 10_000 })
  })

  test('input is disabled while context is loading', async ({ page }) => {
    // Register a slow roster validation response
    await page.route('**/api/roster/*/validation', async route => {
      await new Promise(resolve => setTimeout(resolve, 2000))
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          code: 200,
          msg: 'Validation retrieved',
          data: {
            validationId: '55555555-5555-5555-5555-555555555555',
            status: 'completed',
            totalShifts: 1,
            passedShifts: 1,
            failedShifts: 0,
            totalIssues: 0,
            criticalIssues: 0,
            affectedEmployees: 0,
            weekStartDate: '2026-02-23',
            weekEndDate: '2026-03-01',
            totalEmployees: 1,
            validatedAt: '2026-02-26T00:00:00Z',
            failureType: 'Compliance',
            retriable: false,
            issues: [],
          },
        }),
      })
    })

    await page.goto(
      '/fairbot?intent=roster&rosterId=55555555-5555-5555-5555-555555555555'
    )

    // While context is loading, input should be disabled
    await expect(page.getByText('Loading roster context...')).toBeVisible({
      timeout: 5_000,
    })
    const input = messageInput(page)
    await expect(input).toBeDisabled()

    // After loading completes, input should be enabled
    await expect(page.getByText('Roster context loaded')).toBeVisible({
      timeout: 10_000,
    })
    await expect(input).toBeEnabled()
  })
})
