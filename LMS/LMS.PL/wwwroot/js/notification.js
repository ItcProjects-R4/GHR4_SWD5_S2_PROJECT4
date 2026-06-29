const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

// Start the connection
connection.start().then(function () {
    console.log("Connected to Notification Hub");
}).catch(function (err) {
    return console.error(err.toString());
});

document.addEventListener("DOMContentLoaded", function () {
    const initialSpan = document.getElementById("initialUnreadCount");
    if (initialSpan) {
        const count = parseInt(initialSpan.getAttribute("data-count")) || 0;
        if (count > 0) {
            const badge = document.getElementById("notificationBadge");
            if (badge) {
                badge.innerText = count;
                badge.style.display = "block";
            }
        }
    }
});

// Listen for notifications
connection.on("ReceiveNotification", function (notification) {
    console.log("New Notification:", notification);

    const noNotifMsg = document.getElementById("noNotificationsMsg");
    if (noNotifMsg) {
        noNotifMsg.style.display = "none";
    }

    const notificationList = document.getElementById("notificationList");
    
    // Create new list item
    const li = document.createElement("li");
    li.className = "p-3 border-bottom unread-notification";
    li.style.backgroundColor = "#f8f9fa"; // light gray background for unread

    let iconClass = "bi-info-circle text-primary";
    if (notification.type === 1) iconClass = "bi-bag-check text-success";
    if (notification.type === 2) iconClass = "bi-exclamation-triangle text-warning";
    if (notification.type === 3) iconClass = "bi-journal-check text-info";

    li.innerHTML = `
        <div class="d-flex align-items-start gap-2">
            <i class="bi ${iconClass} mt-1"></i>
            <div>
                <h6 class="mb-1" style="font-size: 13px; font-weight: 600;">${notification.title}</h6>
                <p class="mb-1 text-muted" style="font-size: 12px; line-height: 1.4;">${notification.message}</p>
                <small class="text-secondary" style="font-size: 11px;">Just now</small>
            </div>
        </div>
    `;

    // Insert right after the header
    const headerLi = notificationList.querySelector("li.sticky-top");
    if (headerLi && headerLi.nextSibling) {
        notificationList.insertBefore(li, headerLi.nextSibling);
    } else {
        notificationList.appendChild(li);
    }

    // Update badge count
    const badge = document.getElementById("notificationBadge");
    if (badge) {
        let count = parseInt(badge.innerText) || 0;
        badge.innerText = count + 1;
        badge.style.display = "block";
    }
});
