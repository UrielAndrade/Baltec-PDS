## 2026-09-23 - Accessible Password Visibility Toggles in Blazor
**Learning:** Clickable `<span>` elements used as input action icons (such as eye icons for password toggle) are unreachable via keyboard navigation and invisible to screen readers without interactive semantics.
**Action:** Replace `<span>` toggle wrappers with `<button type="button" class="input-icon" aria-label="..." aria-pressed="...">` and add explicit `:focus-visible` ring styles.
