
$(document).ready()
{
    //Remove conflix bootstrap jquery dialog x on close button
    $.fn.bootstrapBtn = $.fn.button.noConflict();

    if (sessionStorage.getItem('accessToken') === null) {
        window.location.href = '../Registration_Login/login.html';
    }

    //Events
    //-------------------------------------------------------------------------------------
    $('#divEditDialog').hide();

    $('#tokenExpiryModal').on('hidden.bs.modal', function () {
        window.location.href = '../Registration_Login/login.html';
    })

    $('#linkCollapseValidationError').click(function () {
        $('#validationError').hide('fade');
    });

    $('#btnLoadProductsByCompany').click(function () {
        retrieveJsonProducts(100, 0, $('#selectCompany option:selected').text());
    });

    $('#btnLogout').click(function () {
        // logout
        sessionStorage.removeItem('accessToken');
        sessionStorage.removeItem('identity')
        window.location.href = '../Registration_Login/Login.html'
    });

    $('.form-control.eric').change(function () {
        var end = this.value;
       
    });

    $('#tblProductsByCompany tbody').on('click', 'tr', function (args) {

        //grab the clicked row index
        var rowIndex = $(this).closest('tr').prevAll().length;

        //using row index get a colomn array from data table
        var columns = new $.fn.dataTable.Api('#tblProductsByCompany').rows(rowIndex).data();

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
            $('.selectpicker').clone().appendTo($('#tblEditDialog .companyName'));
            $('#tblEditDialog .companyName').children(0).addClass("eric");


            $('#txtId').val(columns[0].ProductId);
            $('#tblEditDialog .companyName').children(0)[0].selectedIndex = $("#selectCompany")[0].selectedIndex;

            //set the selected option using the productCategoryId from datatabel row.
            $('#selectCategory').val(columns[0].ProductCategoryId).attr('selected', 'selected');

            $('#txtProductName').val(columns[0].ProductName);
            $('#txtDescription').val(columns[0].Description);
            $('#txtPrice').val(columns[0].Price);

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
                        var dialogSelectedCompany = $('.selectpicker.form-control.eric').find(':selected').val()
                        $(this).dialog("close");
                        //for some reason the opening of dialog removes row hightlight select class
                        //when dialog dismiss find row that has edit button on it an re-add the select class
                        table.$('tr').each(function () {
                            if ($(this).find('td:nth-child(1)').children(1).length > 0)
                                $(this).addClass('selected');
                        });
                        saveProduct(dialogSelectedCompany);
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


    //function calls
    //------------------------------------------------------------------------------------
    initLogin();

    initProductsByCompanyTable();

    loadCompanyNames();

    loadProductCategoryNames();

    //classes
    // ---------------------------------------------------------------------------------------------------
    //Class for holing and getting web api service end points
    function serviceEndPoints(isProduction) {

        var baseDev = 'http://localhost:55749/';
        var baseProd = 'http://localhost:8081/';
        var product = 'async/api/products';
        var company = 'async/api/companies';
        var productCategory = 'async/api/productCategories';
        var productByCompany = 'async/api/products/getPagedByCompany';
        var base = isProduction == true ? baseProd : baseDev;

        this.productUrl = function () {
            return base + product;
        }
        this.companyUrl = function () {
            return base + company;
        }
        this.productCategory = function () {
            return base + productCategory;
        }
        this.productByCompanyUrl = function () {
            return base + productByCompany;
        }
    };
    //functions
    //---------------------------------------------------------------------------------------
    function loadCompanyNames() {
        var uri = new serviceEndPoints(window.IsProductionMode).companyUrl();
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

                $('#errorMessage').text(jqXHR.status + ' ' + jqXHR.responseText + ' ' + jqXHR.statusText);
                $('#validationError').show('fade');
            }
        });
    };

    function initLogin() {
        // if not logged in redirect to login
        if (sessionStorage.getItem('accessToken') == null)
            window.location.href = '../Registration_Login/Login.html';
        else
            // show user identity
            $('#spanIdentity').text('Welcome ' + sessionStorage.getItem('identity'));
    }

    function loadProductCategoryNames() {
        var uri = new serviceEndPoints(window.IsProductionMode).productCategory();
        $.ajax({
            method: 'GET',
            url: uri,
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            success: function (data) {
                $.each(data, function (index, value) {
                    var row = $('<option value=' + value.ProductCategoryId + '>' + value.CategoryName + '</option>');
                    $('#selectCategory').append(row);
                });
            },
            error: function (jqXHR) {
                // API returns error in jQuery xml http request object.

                $('#errorMessage').text(jqXHR.status + ' ' + jqXHR.responseText + ' ' + jqXHR.statusText);
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
                { 'data': 'Price' },
                { 'data': 'CategoryName' },
                { 'data': 'CompanyId' },
                { 'data': 'ProductCategoryId' }
            ],

            "bAutoWidth": false,
            "columnDefs": [
                { 'type': 'numeric-comma', 'targets': 4 },
                { 'visible': false, 'targets': 6 },
                { 'visible': false, 'targets': 7 }
            ],

            "aaSorting": [],
            "fnFooterCallback": function (nFoot, aData, iStart, iEnd, aiDisplay) {
            },
            "createdRow": function (row, data, index) {
                $('td', row).eq(4).addClass('enMoney');
            }
        });
    };

    function retrieveJsonProducts(displayLength, displayStart, companyName) {

        var uri = new serviceEndPoints(window.IsProductionMode).productByCompanyUrl() + '?CompanyName=' + companyName + '&DisplayLength=' + displayLength + '&DisplayStart=' + displayStart + '&SortColumn=2&SortDirection=asc'

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
                    $('#errorMessage').text(jqXHR.status + ' ' + jqXHR.responseText + ' ' + jqXHR.statusText);
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

    function saveProduct(clonedSelectedCompanyFromDialog) {

        var productToSave = {};
        productToSave.ProductId = $('#txtId').val();
        productToSave.ProductName = $('#txtProductName').val();
        productToSave.Description = $('#txtDescription').val();
        //remove thousands seperator that was added via css for display
        productToSave.Price = parseFloat($('#txtPrice').val().replace(/,/g, ''));
        productToSave.CompanyId = clonedSelectedCompanyFromDialog;
        productToSave.ProductCategoryId = $('#selectCategory').find(':selected').val();
       
        var serializeProduct = JSON.stringify(productToSave);

        var uri = new serviceEndPoints(window.IsProductionMode).productUrl();;
        $.ajax({
            url: uri,
            method: 'PUT',
            contentType: 'application/json; charset=utf-8',
            //serialize productToSave
            data: serializeProduct,
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            success: function (data) {
                //cannot use id selector because this selector object was cloned so both original and clone have same Id. 
                var currentCompany = $('.selectpicker').find(':selected').val()
                retrieveJsonProducts(100, 1, currentCompany)
            },
            error: function (jqXHR) {
                $('#errorMessage').text(jqXHR.status + ' ' + jqXHR.responseText + ' ' + jqXHR.statusText);
                $('#validationError').show('fade');
            }
        });
    }
};
