$(document).ready()
{
    //Remove conflix bootstrap jquery dialog x on close button
    $.fn.bootstrapBtn = $.fn.button.noConflict();

    $('#divEditDialog').hide();

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
        retrieveJsonProducts(100, 0, $('#selectCompany option:selected').text());
    });

    $('#tblProductsByCompany tbody').on('click', 'tr', function () {

        //grap all the column values for the selected row
        var tds = $(this).find('td');
        if (tds.length != 0) {
            var id = tds.eq(0).text();
            var companyName = tds.eq(1).selected;
            var selectedCompanyIndex = $("#selectCompany")[0].selectedIndex;
            var productName = tds.eq(2).text();
            var description = tds.eq(3).text();
            var price = tds.eq(4).text();
        }

        //highlight the selected row and only the selected row
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }

        //remove any edit row buttons that have already been added
        table.$('tr').each(function () {
            $(this).find('td:nth-child(1)').children(1).remove();
        });

        //add a new editrow button on clicked row 
        $(this).find('td:nth-child(1)').prepend('<input type="button" id="btnEditRow" value="Edit" class="btn btn-success" />' + ' ');

        //add anonymous function for editrow button click to load and show the dialog table
        $(this).find('td:nth-child(1)').children(1).on('click', function () {

            //find company name td based on class and remove selectCompany dropdown if it exists
            if ($('#tblEditDialog .companyName').children().length > 0) {
                $('#tblEditDialog .companyName').children().remove();
            }
            //make a deep copy of the selectCompany dropdown from above and inject into tblEditDialog td companyName
            $('.selectpicker').clone().appendTo($('#tblEditDialog .companyName'))

            $('#txtId').val(id);
            $('#tblEditDialog .companyName').children(0)[0].selectedIndex = selectedCompanyIndex;
            $('#txtProductName').val(productName);
            $('#txtDescription').val(description);
            $('#txtPrice').val(price);


            //Show the dialog for editing row column values
            $('#divEditDialog').show('fade');
            $('#divEditDialog').dialog({
                title: 'Edit Row',
                resizable: false,
                height: "auto",
                width: 500,
                modal: true,
                buttons: {
                    "Save": function () {
                        $(this).dialog("close");
                        //for some reason the opening of dialog removes row hightlight select class
                        //when dialog dismiss find row that has edit button on it an re-add the select class
                        table.$('tr').each(function () {
                            if ($(this).find('td:nth-child(1)').children(1).length > 0)
                                $(this).addClass('selected');
                        });
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                        //for some reason the opening of dialog removes row hightlight select class
                        //when dialog dismiss find row that has edit button on it an re-add the select class
                        table.$('tr').each(function () {
                            if ($(this).find('td:nth-child(1)').children(1).length > 0)
                                $(this).addClass('selected');
                        });
                    },
                }
            });
        });
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
                    var row = $('<option value=' + value.CompanyId + '>' + value.CompanyName + '</option>');
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

    function retrieveJsonProducts(displayLength, displayStart, companyName) {

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
