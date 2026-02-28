import type {
  AgentActionPlanData,
  AgentChatSource,
} from '@/services/fairbotApi'
import type {
  FairBotActionFollowUp,
  FairBotActionPlan,
  FairBotActionPlanItem,
  FairBotActionShift,
  FairBotMessageMetadata,
} from '../types/fairbot.types'

export const toAssistantMetadata = (
  model?: string,
  note?: string | null,
  sources?: AgentChatSource[],
  actionPlanData?: AgentActionPlanData | null
): FairBotMessageMetadata | undefined => {
  const normalizedSources = (sources ?? [])
    .filter(item => Boolean(item?.source))
    .map(item => ({
      source: item.source,
      page: item.page,
      content: item.content,
    }))

  const toActionShift = (value: unknown): FairBotActionShift | null => {
    if (!value || typeof value !== 'object') {
      return null
    }
    const source = value as Record<string, unknown>
    return {
      employee: String(source.employee ?? ''),
      dates: String(source.dates ?? ''),
      description: String(source.description ?? ''),
    }
  }

  const toActionPlanItem = (value: unknown): FairBotActionPlanItem | null => {
    if (!value || typeof value !== 'object') {
      return null
    }
    const source = value as Record<string, unknown>
    const affected = Array.isArray(source.affected_shifts)
      ? source.affected_shifts
          .map(toActionShift)
          .filter((item): item is FairBotActionShift => item !== null)
      : []

    const id = String(source.id ?? '')
    const title = String(source.title ?? '')
    if (!id || !title) {
      return null
    }

    return {
      id,
      priority: String(source.priority ?? ''),
      title,
      owner: String(source.owner ?? ''),
      checkType: String(source.check_type ?? ''),
      issueCount:
        typeof source.issue_count === 'number' ? source.issue_count : 0,
      criticalCount:
        typeof source.critical_count === 'number' ? source.critical_count : 0,
      affectedShifts: affected,
      whatToChange: String(source.what_to_change ?? ''),
      why: String(source.why ?? ''),
      expectedOutcome: String(source.expected_outcome ?? ''),
      riskIfIgnored: String(source.risk_if_ignored ?? ''),
      focusExamples: String(source.focus_examples ?? ''),
    }
  }

  const toFollowUp = (value: unknown): FairBotActionFollowUp | null => {
    if (!value || typeof value !== 'object') {
      return null
    }
    const source = value as Record<string, unknown>
    const id = String(source.id ?? '')
    const label = String(source.label ?? '')
    const prompt = String(source.prompt ?? '')
    if (!id || !label || !prompt) {
      return null
    }

    return {
      id,
      label,
      prompt,
      actionId: String(source.action_id ?? ''),
    }
  }

  const toActionPlan = (
    payload?: AgentActionPlanData | null
  ): FairBotActionPlan | undefined => {
    if (!payload || typeof payload !== 'object') {
      return undefined
    }

    const actions = Array.isArray(payload.actions)
      ? payload.actions
          .map(toActionPlanItem)
          .filter((item): item is FairBotActionPlanItem => item !== null)
      : []
    if (actions.length === 0) {
      return undefined
    }

    const quickFollowUps = Array.isArray(payload.quick_follow_ups)
      ? payload.quick_follow_ups
          .map(toFollowUp)
          .filter((item): item is FairBotActionFollowUp => item !== null)
      : []
    const fallbackTemplates: Array<{
      labelSuffix: string
      buildPrompt: (title: string) => string
    }> = [
      {
        labelSuffix: 'Shift edits',
        buildPrompt: title =>
          `For ${title}, provide concrete shift-level edits using only the affected shifts. Output per employee/date: before -> after, and explain why each edit resolves the issue.`,
      },
      {
        labelSuffix: 'Manager checklist',
        buildPrompt: title =>
          `For ${title}, create an execution checklist for the roster manager. Include sequencing, owner, and completion criteria for each step.`,
      },
      {
        labelSuffix: 'Validation checks',
        buildPrompt: title =>
          `For ${title}, list the exact validation checks to run after schedule changes. Include pass criteria and what to do if a check still fails.`,
      },
    ]

    const fallbackFollowUps: FairBotActionFollowUp[] = actions
      .slice(0, 3)
      .map((action, index) => {
        const template = fallbackTemplates[index % fallbackTemplates.length]
        return {
          id: `fallback_follow_up_${index + 1}`,
          label: `${action.priority || `P${index + 1}`} ${template.labelSuffix}`,
          prompt: template.buildPrompt(action.title),
          actionId: action.id,
        }
      })

    return {
      title: payload.title || 'Top actions to fix this roster',
      validationId: payload.validation_id ?? null,
      actions,
      quickFollowUps:
        quickFollowUps.length > 0 ? quickFollowUps : fallbackFollowUps,
    }
  }

  const actionPlan = toActionPlan(actionPlanData)
  const hasData =
    Boolean(model) ||
    Boolean(note) ||
    normalizedSources.length > 0 ||
    Boolean(actionPlan)
  if (!hasData) {
    return undefined
  }

  return {
    model,
    note: note ?? null,
    sources: normalizedSources,
    actionPlan,
  }
}
