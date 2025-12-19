import type { Meta, StoryObj } from '@storybook/react'
import { StorybookSmoke } from './StorybookSmoke'

const meta: Meta<typeof StorybookSmoke> = {
  title: 'Dev/StorybookSmoke',
  component: StorybookSmoke,
}
export default meta

type Story = StoryObj<typeof StorybookSmoke>

export const Default: Story = {}
