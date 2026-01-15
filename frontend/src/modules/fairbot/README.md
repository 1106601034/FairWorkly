# FairBot Module

## Purpose

FairBot provides a chat-style assistant UI for Fair Work compliance help. It lets users send messages, upload roster/payroll files, and view a quick summary of results in a side panel.

## File Structure

```Structure
src/modules/fairbot/
  constants/
    fairbot.constants.ts
  features/
    conversation/
      FileUploadZone.tsx
      MessageInput.tsx
      MessageList.tsx
      useConversation.ts
    quickActions/
      actions.config.ts
      QuickActions.tsx
    resultsPanel/
      PayrollSummary.tsx
      QuickSummary.tsx
      ResultsEmpty.tsx
      ResultsPanel.tsx
      RosterSummary.tsx
  hooks/
    useFairBot.ts
    useFileUpload.ts
    useMessageStream.ts
    usePermissions.ts      # Deprecated - use @/shared/hooks/usePermissions
    useResultsPanel.ts
  pages/
    FairBotChat.tsx
  types/
    fairbot.types.ts
  ui/
    MessageBubble.tsx
    TypingIndicator.tsx
    WelcomeMessage.tsx
```

For shared UI, hooks, and constants referenced here, see `src/shared/README.md`.

## Entry Point and Routing

- Route: `src/app/routes/fairbot.routes.tsx` maps `/fairbot` to the page.
- Page component: `src/modules/fairbot/pages/FairBotChat.tsx`
- Access control: `fairbot.routes.tsx` currently renders `FairBotChat` directly; wrap with
  `ProtectedRoute` once auth is wired.

## UI Composition (FairBotChat)

- Sidebar column
  - `Sidebar` from `src/shared/components/layout` (navigation)
- Chat column
  - `WelcomeMessage` (intro + bullet list)
  - `QuickActions` (prebuilt actions + optional file upload)
  - `MessageList` + `TypingIndicator`
  - `FileUploadZone` (drag/drop + browse)
  - `MessageInput` (text input + send)
- Results column
  - `ResultsPanel` (summary or empty state)

Note: `FairBotChat` renders `Sidebar` directly to achieve a three-column layout.
If `MainLayout` is wired in later, avoid double sidebars.

## Core Data Flow

1. User submits a message (MessageInput or QuickActions).
2. `useConversation` delegates to `useFairBot`.
3. `useFairBot` creates a user message, simulates a response, and appends an assistant message.
4. If a summary is detected, `useResultsPanel` updates the results state.
5. Conversation and results are persisted in `sessionStorage`.

## Key Hooks

- `useFairBot`
  - Owns message state, mock response building, and error handling.
  - Persists conversation (files are stripped before save).
- `useConversation`
  - View-model wrapper for messages, typing, and errors.
- `useMessageStream`
  - Controls typing indicator delays while loading.
- `useFileUpload`
  - Drag/drop + file picker validation.
  - Accepts `.csv` and `.xlsx`, max 5MB.
  - Returns `{ inputRef, controls }` where `controls` is ref-free render state and handlers.
- `useResultsPanel`
  - Reads/writes current results to `sessionStorage`.
- `usePermissions` (from `@/shared/hooks/usePermissions`)
  - Shared permission hook providing `hasPermission()` and `canAccessModule()`.
  - FairBot quick actions use this to filter visible actions based on user permissions.
  - See `src/shared/types/permissions.types.ts` for available `Permission` values.

Note: `FileUploadZone` receives `inputRef` separately to avoid ref-like props in render.

## Quick Actions

- Config lives in `features/quickActions/actions.config.ts`.
- Each action can:
  - Pre-fill an initial message.
  - Require a file upload.
  - Gate visibility via `requiredPermission` using the shared `Permission` enum.
- Permission values are defined in `FAIRBOT_QUICK_ACTIONS.PERMISSIONS` (uses shared `Permission`).
- Actions with `requiredPermission: null` are visible to all authenticated users.

## Results Panel

- `ResultsPanel` renders:
  - `QuickSummary` when results exist.
  - `ResultsEmpty` otherwise.
- Summary cards:
  - `PayrollSummary` and `RosterSummary` show stats and top issues.
  - Navigates via `FAIRBOT_ROUTES`:
    - `/payroll/upload`, `/compliance/upload`, `/my-profile`, `/documents`.

## Constants and Types

- Strings, layout values, timing, and IDs: `constants/fairbot.constants.ts`.
- Type contracts: `types/fairbot.types.ts`.

## Extending the Module

- Add a new quick action:
  - Update `FAIRBOT_QUICK_ACTIONS` constants.
  - Add to `features/quickActions/actions.config.ts`.
  - Map an icon in `QuickActions.tsx`.
- Add a new result type:
  - Extend `FairBotResult` union in `fairbot.types.ts`.
  - Update `FAIRBOT_RESULTS` and `FAIRBOT_MOCK_DATA`.
  - Add rendering to `QuickSummary`.
- Connect real agent service:
  - Replace `buildMockResponse` in `useFairBot` with an API call returning
    `FairBotAgentResponse`.

## Testing Notes

- No tests exist yet.
- Add colocated `*.test.tsx` for key flows:
  - Message sending, file validation, summary rendering, typing indicator timing.

## Current Limitations

- Responses and summaries use mock data.
- Permission checks use default role-based mappings (backend API not yet integrated).
- Files are not persisted to session storage.

## Permission System

FairBot uses the shared permission system from `@/shared/hooks/usePermissions`:

```typescript
import { usePermissions } from '@/shared/hooks/usePermissions'
import { Permission } from '@/shared/types/permissions.types'

const { hasPermission } = usePermissions()
if (hasPermission(Permission.CheckPayrollCompliance)) {
  // User can access payroll compliance features
}
```

Quick action permissions:
- `CheckPayrollCompliance` - Required for payroll check action (admin only)
- `CheckRosterCompliance` - Required for roster check action (admin + manager)
- `ManageDocuments` - Required for contract generation action (admin only)
- `null` - "Ask a Question" action is available to all users
