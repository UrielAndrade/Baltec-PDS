## 2026-03-30 - Interactive Controls on Non-Semantic Elements in Blazor Components
**Learning:** In Blazor forms, attaching `@onclick` event handlers to `<span>` or `<div>` elements (such as password show/hide eye icons) leaves them completely inaccessible to keyboard users (no focus state or Enter/Space trigger) and invisible to screen readers without ARIA roles or labels.
**Action:** Always replace click-handled wrapper `<span>`/`<div>` elements with `<button type="button">` having dynamic `aria-label`/`title` attributes and explicit `:focus-visible` styles.
