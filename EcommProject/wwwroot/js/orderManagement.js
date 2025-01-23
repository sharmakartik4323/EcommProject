var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#allOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/GetAll",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#pendingOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/PendingOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#approvedOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/ApprovedOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#processingOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/ProcessingOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#shippedOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/ShippedOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#cancelledOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/CancelledOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
    dataTable = $('#refundedOrders').DataTable({
        "ajax": {
            "url": "/Admin/OrderManagement/RefundedOrders",
            "type": "GET"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "orderDate", "width": "30%" },
            { "data": "name", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/OrderManagement/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
}
