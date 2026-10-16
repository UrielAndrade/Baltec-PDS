## 2026-03-30 - Password Toggle Button Accessibility in Blazor Inputs
**Learning:** Interactive toggle elements implemented with non-semantic `<span>` elements lack keyboard focus, screen reader announcements, and proper toggle states.
**Action:** Replace `<span>` toggle controls with native `<button type="button">` elements containing proper `aria-label`, `aria-pressed`, and `title` attributes.
