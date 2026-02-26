import { test, expect, type Page } from '@playwright/test'

async function mockAuthenticatedAdmin(page: Page) {
  const accessToken = 'e2e-access-token'
  const user = {
    id: 'e2e-user-id',
    email: 'admin@fairworkly.com.au',
    firstName: 'E2E',
    lastName: 'Admin',
    role: 'Admin',
    organizationId: 'e2e-org-id',
  }

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

  await page.route('**/api/auth/me', async route => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({
        code: 200,
        msg: 'Current user',
        data: user,
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
  return page.getByPlaceholder('Ask a Fair Work question...')
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
      const body = route.request().postData() ?? ''
      expect(body).toContain('roster_explain')
      expect(body).toContain('roster_validation')
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(
          asFairBotEnvelope('Explained using roster context.')
        ),
      })
    })

    await page.goto(
      `/fairbot?intent=roster&rosterId=${rosterId}&validationId=${validationId}`
    )
    await expect(page.getByText('Roster context loaded')).toBeVisible({
      timeout: 15_000,
    })

    const input = messageInput(page)
    await input.fill('Please explain this roster result.')
    await page.getByRole('button', { name: 'Send message' }).click()

    await expect(page.getByText('Explained using roster context.')).toBeVisible(
      {
        timeout: 10_000,
      }
    )
  })

  test('explain fallback retries as general when roster context is required', async ({
    page,
  }) => {
    const rosterId = '33333333-3333-3333-3333-333333333333'
    const validationId = '44444444-4444-4444-4444-444444444444'
    let chatRequestCount = 0
    const requestBodies: string[] = []

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
    ).toBeVisible({
      timeout: 10_000,
    })
    await expect.poll(() => chatRequestCount, { timeout: 10_000 }).toBe(2)
    expect(requestBodies[0]).toContain('roster')
    expect(requestBodies[1]).toContain('compliance')
  })
})
