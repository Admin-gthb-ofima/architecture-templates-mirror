(() => {
  const body = document.body;
  const toggleButton = document.querySelector("[data-menu-toggle]");
  const overlay = document.querySelector("[data-menu-close]");

  const closeMenu = () => body.classList.remove("menu-open");

  toggleButton?.addEventListener("click", () => {
    body.classList.toggle("menu-open");
  });

  overlay?.addEventListener("click", closeMenu);

  window.addEventListener("resize", () => {
    if (window.innerWidth > 980) {
      closeMenu();
    }
  });
})();
