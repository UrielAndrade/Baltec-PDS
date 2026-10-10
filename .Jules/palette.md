# Palette's Journal - UX & Accessibility Learnings

## 2025-05-18 - Blazor Modal & Search Accessibility Improvements
**Learning:** In Blazor modal dialogs and search inputs, icon-only buttons (like `&times;` for closing or clearing) often lack accessible names (`aria-label`) and modal dialog containers lack proper ARIA attributes (`role="dialog"`, `aria-modal="true"`, `aria-labelledby`), causing screen reader accessibility gaps.
**Action:** Always add explicit `aria-label` to clear/close buttons and structure Blazor modals with `role="dialog"`, `aria-modal="true"`, and associated heading IDs.
