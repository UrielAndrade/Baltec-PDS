# Palette's Journal - Critical UX Learnings

## 2026-10-03 - Accessible Password Visibility Toggles
**Learning:** Password visibility icons placed inside custom inputs are often implemented as non-semantic `<span>` or `<div>` elements, preventing keyboard accessibility (tab navigation & Enter/Space triggering) and screen reader support. Changing them to `<button type="button">` with dynamic `aria-label` ("Mostrar senha" / "Ocultar senha") and setting right padding on the input field ensures accessible, smooth UX without layout breakage.
**Action:** Always wrap password toggle icons in semantic `<button type="button">` elements with explicit focus states and text padding to prevent text overlay.
