# Palette's Journal - Critical UX & Accessibility Learnings

## 2026-03-30 - Password Visibility Toggles Accessibility in Blazor Forms
**Learning:** In forms using broad button styling (e.g., `.login-form button` or `.cadastro-form button`), password visibility toggles implemented as clickable `<span>` elements lack keyboard focusability, ARIA state (`aria-pressed`, dynamic `aria-label`), and screen reader semantics. Replacing them with `<button type="button">` requires scoping the general button selectors with `:not(.input-icon)` or explicitly resetting dimensions, backgrounds, and transforms inside `.password-wrapper button.input-icon` with `:focus-visible` outline support.
**Action:** Always implement input toggle controls as `<button type="button">` with dynamic `aria-label`, `aria-pressed`, decorative inner icon (`alt="" aria-hidden="true"`), and safeguard form CSS with `:not(.input-icon)`.
