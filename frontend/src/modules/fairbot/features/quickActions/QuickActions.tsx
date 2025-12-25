import type { ElementType } from 'react'
import { styled } from '@mui/material/styles'
import ButtonBase from '@mui/material/ButtonBase'
import Typography from '@mui/material/Typography'
import AttachMoneyOutlined from '@mui/icons-material/AttachMoneyOutlined'
import EventNoteOutlined from '@mui/icons-material/EventNoteOutlined'
import HelpOutlineOutlined from '@mui/icons-material/HelpOutlineOutlined'
import DescriptionOutlined from '@mui/icons-material/DescriptionOutlined'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_NUMBERS,
  FAIRBOT_QUICK_ACTIONS,
  FAIRBOT_QUICK_ACTIONS_UI,
  FAIRBOT_TEXT,
} from '../../constants/fairbot.constants'
import type { FairBotPermission, FairBotQuickAction } from '../../types/fairbot.types'
import type { FileUploadControls } from '../../hooks/useFileUpload'
import { usePermissions } from '../../hooks/usePermissions'
import { quickActionLabels, quickActions } from './actions.config'

type PaletteKey = 'info' | 'success' | 'secondary' | 'warning'
interface ActionPaletteKeys {
  background: PaletteKey
  border: PaletteKey
}

interface QuickActionsProps {
  upload: FileUploadControls
  onSendMessage: (message: string) => Promise<void>
  actions?: FairBotQuickAction[]
}

interface ActionCardProps {
  accent: PaletteKey
  isFullSpan: boolean
}

const ICON_COMPONENTS: Record<string, ElementType> = {
  [FAIRBOT_QUICK_ACTIONS.ICONS.PAYROLL]: AttachMoneyOutlined,
  [FAIRBOT_QUICK_ACTIONS.ICONS.ROSTER]: EventNoteOutlined,
  [FAIRBOT_QUICK_ACTIONS.ICONS.QUESTION]: HelpOutlineOutlined,
  [FAIRBOT_QUICK_ACTIONS.ICONS.DOCUMENT]: DescriptionOutlined,
}

const ActionsSection = styled('section')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.QUICK_ACTIONS_GAP}px`,
})

const ActionsHeader = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightMedium,
}))

const ActionsGrid = styled('div')({
  display: 'grid',
  gap: `${FAIRBOT_LAYOUT.QUICK_ACTIONS_GAP}px`,
  gridTemplateColumns: `repeat(${FAIRBOT_LAYOUT.QUICK_ACTIONS_COLUMNS}, minmax(0, 1fr))`,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: `repeat(${FAIRBOT_NUMBERS.ONE}, minmax(0, 1fr))`,
  },
})

const ActionCard = styled(ButtonBase, {
  shouldForwardProp: (prop) => prop !== 'accent' && prop !== 'isFullSpan',
})<ActionCardProps>(({ theme, accent, isFullSpan }) => ({
  borderRadius: `${FAIRBOT_QUICK_ACTIONS_UI.CARD_RADIUS}px`,
  padding: `${FAIRBOT_QUICK_ACTIONS_UI.CARD_PADDING}px`,
  minHeight: `${FAIRBOT_QUICK_ACTIONS_UI.CARD_MIN_HEIGHT}px`,
  textAlign: 'left',
  borderStyle: 'solid',
  borderWidth: `${FAIRBOT_QUICK_ACTIONS_UI.CARD_BORDER_WIDTH}px`,
  borderColor: theme.palette[accent].light,
  backgroundColor: theme.palette.background.paper,
  display: 'flex',
  alignItems: 'center',
  gap: `${FAIRBOT_LAYOUT.QUICK_ACTIONS_GAP}px`,
  justifyContent: 'flex-start',
  gridColumn: isFullSpan ? FAIRBOT_TEXT.FULL_SPAN : undefined,
  transition: theme.transitions.create(['box-shadow', 'transform'], {
    duration: FAIRBOT_QUICK_ACTIONS_UI.TRANSITION_MS,
  }),
  '&:hover': {
    boxShadow: theme.shadows[FAIRBOT_NUMBERS.TWO],
    transform: `translateY(${FAIRBOT_QUICK_ACTIONS_UI.HOVER_TRANSLATE_Y_PX}px)`,
  },
}))

const IconBadge = styled('span')<{
  accent: PaletteKey
}>(({ theme, accent }) => ({
  width: `${FAIRBOT_QUICK_ACTIONS_UI.ICON_SIZE * FAIRBOT_NUMBERS.TWO}px`,
  height: `${FAIRBOT_QUICK_ACTIONS_UI.ICON_SIZE * FAIRBOT_NUMBERS.TWO}px`,
  borderRadius: '50%',
  display: 'inline-flex',
  alignItems: 'center',
  justifyContent: 'center',
  backgroundColor: theme.palette[accent].light,
  color: theme.palette[accent].dark,
  flexShrink: 0,
}))

const ActionContent = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_QUICK_ACTIONS_UI.CONTENT_GAP}px`,
})

const ActionTitle = styled(Typography)(({ theme }) => ({
  fontWeight: theme.typography.fontWeightMedium,
}))

const ActionDescription = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
}))

const resolvePaletteKeys = (color: string): ActionPaletteKeys => {
  const fallback =
    FAIRBOT_QUICK_ACTIONS_UI.COLOR_MAP[FAIRBOT_QUICK_ACTIONS.COLORS.BLUE]
  return (
    FAIRBOT_QUICK_ACTIONS_UI.COLOR_MAP[
      color as keyof typeof FAIRBOT_QUICK_ACTIONS_UI.COLOR_MAP
    ] ?? fallback
  ) as ActionPaletteKeys
}

const hasPermissionForAction = (
  hasPermission: (permission: FairBotPermission | null) => boolean,
  action: FairBotQuickAction,
) => {
  return !action.requiredPermission || hasPermission(action.requiredPermission)
}

export const QuickActions = ({
  upload,
  onSendMessage,
  actions = quickActions,
}: QuickActionsProps) => {
  const { hasPermission } = usePermissions()
  // Hide actions when the current user lacks the required permission.
  const visibleActions = actions.filter((action) =>
    hasPermissionForAction(hasPermission, action),
  )

  if (visibleActions.length === FAIRBOT_NUMBERS.ZERO) {
    return null
  }

  // If there is an odd count, let the last card span full width for balance.
  const actionCountIsOdd =
    visibleActions.length % FAIRBOT_NUMBERS.TWO !== FAIRBOT_NUMBERS.ZERO

  // Send the prefilled message and request a file if the action needs one.
  const handleActionClick = async (action: FairBotQuickAction) => {
    await onSendMessage(action.initialMessage)
    if (action.requiresFile) {
      upload.openFileDialog()
    }
  }

  return (
    <ActionsSection aria-label={FAIRBOT_ARIA.QUICK_ACTIONS}>
      <ActionsHeader variant="subtitle1">
        {quickActionLabels.gridLabel ?? FAIRBOT_LABELS.PROMPT_QUESTION}
      </ActionsHeader>
      <ActionsGrid>
        {visibleActions.map((action, index) => {
          const paletteKeys = resolvePaletteKeys(action.color)
          const IconComponent = ICON_COMPONENTS[action.icon] ?? HelpOutlineOutlined
          const isLast = index === visibleActions.length - FAIRBOT_NUMBERS.ONE
          const isFullSpan = actionCountIsOdd && isLast

          return (
            <ActionCard
              key={action.id}
              accent={paletteKeys.border}
              isFullSpan={isFullSpan}
              aria-label={action.title}
              onClick={() => {
                void handleActionClick(action)
              }}
            >
              <IconBadge accent={paletteKeys.background}>
                <IconComponent fontSize="small" />
              </IconBadge>
              <ActionContent>
                <ActionTitle variant="subtitle1">{action.title}</ActionTitle>
                <ActionDescription variant="body2">
                  {action.description}
                </ActionDescription>
              </ActionContent>
            </ActionCard>
          )
        })}
      </ActionsGrid>
    </ActionsSection>
  )
}
