// Get element
const lumenMouseEl = document.getElementById("util__lumen_mouse");
const excludeTargetEl = document.getElementsByClassName("header")[0];

// Mouse positions
let targetX = window.innerWidth / 2;
let targetY = window.innerHeight / 2;
let mouseX = targetX;
let mouseY = targetY;

// Smoothness
const ease = 0.18;

// Events
window.addEventListener("mousemove", (e) => {
    targetX = e.clientX;
    targetY = e.clientY;

    let mouseEl = document.elementsFromPoint(mouseX, mouseY);
    if (mouseEl.includes(excludeTargetEl))
    {
        lumenMouseEl.classList.add("hidden");
    }
    else
    {
        lumenMouseEl.classList.remove("hidden");
    }
}, {passive: true});
window.addEventListener("mouseleave", () => lumenMouseEl.classList.add("hidden"));
window.addEventListener("blur", () => lumenMouseEl.classList.add("hidden"));
window.addEventListener("mouseenter", () => lumenMouseEl.classList.remove("hidden"));
window.addEventListener("focus", () => lumenMouseEl.classList.remove("hidden"));

// Mouse glow function
function glowAnimation() {
    // lerp
    mouseX += (targetX - mouseX) * ease;
    mouseY += (targetY - mouseY) * ease;

    // Transform element
    lumenMouseEl.style.transform = `translate(${mouseX - (lumenMouseEl.offsetWidth/2)}px, ${mouseY - (lumenMouseEl.offsetHeight/2)}px)`;

    requestAnimationFrame(glowAnimation);
}

requestAnimationFrame(glowAnimation);
