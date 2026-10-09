## 2025-10-09 - Accessible Icon-Only Buttons in Blazor Components
**Learning:** Icon-only interactive elements in Blazor pages (close buttons using `&times;`/`×`, clear-search buttons, and mobile menu toggles) lack accessible names unless an explicit `aria-label` attribute is provided.
**Action:** Always verify that buttons containing only SVGs, HTML entities (`&times;`), or visual symbols include `aria-label` descriptive text for screen reader accessibility.
