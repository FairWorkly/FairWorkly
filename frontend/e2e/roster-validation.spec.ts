import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { test, expect, type Page } from '@playwright/test'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const TEST_DATA_DIR = path.resolve(__dirname, '../../test-data')

async function login(page: Page) {
  await page.goto('/login')
  const emailField = page.getByLabel('Email Address')
  await emailField.waitFor({ state: 'visible', timeout: 15_000 })
  await emailField.fill('admin@fairworkly.com.au')
  await page.getByLabel('Password').fill('fairworkly123')
  await page.locator('form').getByRole('button', { name: 'Sign In' }).click()
  await expect(page).not.toHaveURL(/login/, { timeout: 15_000 })
}

test.describe('Roster validation', () => {
  test.beforeEach(async ({ page }) => {
    await login(page)
    await page.goto('/roster/upload')
    await expect(page.getByText('Upload Roster')).toBeVisible({ timeout: 15_000 })
  })

  test('happy path — upload valid roster and see passing results', async ({
    page,
  }) => {
    // Upload file via the hidden input
    const fileInput = page.locator('input[type="file"]')
    await fileInput.setInputFiles(
      path.join(TEST_DATA_DIR, 'test-happy-path.xlsx'),
    )

    // File card should appear with the filename
    await expect(page.getByText('test-happy-path.xlsx')).toBeVisible()

    // Click "Start Validation" and wait for navigation to results page
    await page.getByRole('button', { name: 'Start Validation' }).click()
    await expect(page).toHaveURL(/\/roster\/results\//, { timeout: 30_000 })

    // Wait for validation to complete (loading spinner disappears)
    await expect(page.getByText('Running compliance checks...')).toBeHidden({
      timeout: 60_000,
    })

    // Summary stat cards should be visible
    await expect(page.getByText('Employees Compliant')).toBeVisible({ timeout: 10_000 })
    await expect(page.getByText('Total Issues Found')).toBeVisible()
    await expect(page.getByText('Critical Issues')).toBeVisible()
    await expect(page.getByText('Employees Affected')).toBeVisible()
  })

  test('compliance errors — upload roster with violations and see issues', async ({
    page,
  }) => {
    // Upload file with compliance errors
    const fileInput = page.locator('input[type="file"]')
    await fileInput.setInputFiles(
      path.join(TEST_DATA_DIR, 'test-compliance-errors.xlsx'),
    )

    await expect(page.getByText('test-compliance-errors.xlsx')).toBeVisible()

    // Start validation
    await page.getByRole('button', { name: 'Start Validation' }).click()
    await expect(page).toHaveURL(/\/roster\/results\//, { timeout: 30_000 })

    // Wait for validation to complete
    await expect(page.getByText('Running compliance checks...')).toBeHidden({
      timeout: 60_000,
    })

    // Summary should show issues
    await expect(page.getByText('Total Issues Found')).toBeVisible({ timeout: 10_000 })

    // Issues by Category section should be rendered with at least one category
    await expect(page.getByText('Issues by Category')).toBeVisible()
    await expect(page.getByText(/\d+ employees? flagged/).first()).toBeVisible()
  })
})
