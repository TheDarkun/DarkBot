// this exists for the soul purpose of re-adding the data-theme back
export function onUpdate() {
    const theme = localStorage.getItem("theme") || "light";
    document.documentElement.setAttribute("data-theme", theme);
}
