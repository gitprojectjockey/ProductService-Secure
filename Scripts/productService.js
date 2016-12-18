$(document).ready()
{
    if (sessionStorage.getItem('accessToken') === null) {
        window.location.href = '../Registration_Login/login.html';
    }

    $('#tokenExpiryModal').on('hidden.bs.modal', function () {
        window.location.href = '../Registration_Login/login.html';
    })


    $('#linkCollapseValidationError').click(function () {
        $('#validationError').hide('fade');
    });

    $('#btnLoadProductsByCompany').click(function () {
        retrieveJson(100, 0, $('#selectCompany option:selected').text());
    });

    initProductsByCompanyTable();

    loadCompanyNames();

    // ---------------------------------------------------------------------------------------------------

    function loadCompanyNames() {
        var uri = 'http://localhost:55749/async/api/companies'
        $.ajax({
            method: 'GET',
            url: uri,
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            success: function (data) {
                $.each(data, function (index, value) {
                    var row = $('<option>' + value.CompanyName + '</option>');
                    $('#selectCompany').append(row);
                });
                $("#selectCompany")[0].selectedIndex = 0
            },
            error: function (jqXHR) {
                // API returns error in jQuery xml http request object.

                $('#errorMessage').text(jqXHR.responseText);
                $('#validationError').show('fade');
            }
        });
    };

    function initProductsByCompanyTable() {
        $('#tblProductsByCompany').dataTable({
            "destroy": true,
            "paging": true,
            "ordering": true,
            "info": true,
            "lengthMenu": [10, 25, 50, 75, 100],
            "columns": [
                { 'data': 'ProductId' },
                { 'data': 'CompanyName' },
                { 'data': 'ProductName' },
                { 'data': 'Description' },
                { 'data': 'Price' }],
            "bAutoWidth": false,
            "columnDefs": [
                { 'type': 'numeric-comma', 'targets': 4 }],
            "aaSorting": [],
            "fnFooterCallback": function (nFoot, aData, iStart, iEnd, aiDisplay) {
            },
            "createdRow": function (row, data, index) {
                $('td', row).eq(4).addClass('enMoney');
            }
        });
    };

    function retrieveJson(displayLength, displayStart, companyName) {

        var uri = 'http://localhost:55749/async/api/products/getpagedByCompany?CompanyName=' + companyName + '&DisplayLength=' + displayLength + '&DisplayStart=' + displayStart + '&SortColumn=2&SortDirection=asc'

        $.ajax({
            method: 'GET',
            url: uri,
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },

            success: function (data) {
                loadProductsByCompanyTable(data);
            },
            error: function (jqXHR) {
                // API returns error in jQuery xml http request object.
                if (jqXHR.status == '401') {
                    $('#tokenExpiryModal').modal('show');
                }
                else {
                    $('#errorMessage').text(jqXHR.responseText);
                    $('#validationError').show('fade');
                }
            }
        });
    };

    function loadProductsByCompanyTable(json) {

        table = $('#tblProductsByCompany').dataTable();
        oSettings = table.fnSettings();
        table.fnClearTable(this);
        //add thousands seperator to Price
        for (var i = 0; i < json.length; i++) {
            json[i].Price = Number(json[i].Price).toLocaleString('en');
        }
       
        for (var i = 0; i < json.length; i++) {
            table.oApi._fnAddData(oSettings, json[i]);
        }

        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
        table.fnDraw();
    }
};
