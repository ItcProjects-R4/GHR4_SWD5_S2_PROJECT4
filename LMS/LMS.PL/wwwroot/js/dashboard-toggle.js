document.addEventListener("DOMContentLoaded", function () {
    const closeBtn = document.getElementById("sidebarCloseToggle");
    const openBtn = document.getElementById("sidebarOpenToggle");
    const container = document.querySelector(".workspace-container") || document.querySelector(".student-workspace-container");
    const sidebar = document.querySelector(".sidebar");

    if (container) {
        // 1. Desktop Collapse State Persistence
        const isCollapsed = localStorage.getItem("sidebar-collapsed") === "true";
        if (isCollapsed && window.innerWidth >= 992) {
            container.classList.add("workspace-collapsed");
        }

        function toggleSidebar(state) {
            if (state) {
                container.classList.add("workspace-collapsed");
            } else {
                container.classList.remove("workspace-collapsed");
            }
            localStorage.setItem("sidebar-collapsed", state);
        }

        if (closeBtn) {
            closeBtn.addEventListener("click", function () {
                toggleSidebar(true);
            });
        }

        if (openBtn) {
            openBtn.addEventListener("click", function () {
                toggleSidebar(false);
            });
        }
    }

    // 2. Mobile Responsive Slide-In Toggle
    const mobileToggle = document.getElementById("sidebar-toggle");
    if (mobileToggle && sidebar) {
        mobileToggle.addEventListener("click", function (e) {
            e.stopPropagation();
            sidebar.classList.toggle("active");
        });
    }

    // Close mobile sidebar when clicking outside of it
    document.addEventListener("click", function (event) {
        if (window.innerWidth < 992 && sidebar && sidebar.classList.contains("active")) {
            if (!sidebar.contains(event.target) && (!mobileToggle || !mobileToggle.contains(event.target))) {
                sidebar.classList.remove("active");
            }
        }
    });
});
