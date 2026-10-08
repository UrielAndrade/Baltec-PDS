# Palette's Journal - UX & Accessibility Learnings

## 2026-03-30 - Accessible Password Visibility Toggles in Blazor
**Learning:** Icon-only password toggles implemented with `<span>` elements lack keyboard tab focus and screen reader accessibility. Replacing them with `<button type="button">` and dynamic `aria-label` ("Mostrar senha" / "Ocultar senha") makes the toggle fully keyboard-accessible and screen-reader friendly without triggering unintended form submissions.
**Action:** Always wrap interactive icon toggles in semantic `<button type="button">` elements with dynamic `aria-label` attributes and `:focus-visible` focus indicators.
