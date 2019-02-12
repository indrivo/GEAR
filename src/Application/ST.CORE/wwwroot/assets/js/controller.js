function List() {
    this.page = 1;
    this.id = '';
    this.perPage = 10;
    this.paginationid = "";
    this.url = location.origin;
    this.method = '';
    this.model = [];
    this.active = 1;
    this.beforeTime = new Date();
    this.afterTime = new Date();
    this.default = {
        "is_success": true,
        "error_keys": [],
        "result": [],
        "model": [],
        "pagination": {
            "total_count": 0,
            "page": 1,
            "total_pages": 0,
            "page_size": 0
        }
    };
    this.template = {
        "name": "",
        "path": "",
        "id": ""
    };
    this.templaterender = null;
    this.templaterenderData = null;
}

List.prototype = {
    constructor: List,
    setPage: function (page) {
        this.page = page;
    },
    setId: function (id) {
        this.id = id;
    },
    setPerPage: function (value) {
        this.perPage = value;
    },
    setPaginationId: function (id) {
        this.paginationid = id;
    },
    setModel: function (model) {
        this.model = model;
    },
    setTemplateSettings: function (model) {
        this.template = model;
    },
    setMethodUrl: function (url) {
        this.method = url;
    },
    StartTime: function () {
        this.beforeTime = new Date();
    },
    EndTime: function () {
        this.afterTime = new Date();
    },
    CalculateTime: function () {
        return ((this.afterTime - this.beforeTime) / 60).toFixed(2);
    },
    getList: function (list) {
        $("#" + list.paginationid).hide();
        list.StartTime();
        $.ajax({
            xhrFields: {
                onprogress: function (e) {
                    $(".windows8").show();
                    if (e.lengthComputable) {
                        //trebuie de facut
                        //daca sint date progresul la cite s-o incarcat
                        console.log(e.loaded / e.total * 100 + '%');
                    }
                }
            },
            url: list.method,
            data: {
                page: list.page,
                pageSize: list.perPage
            },
            method: 'get',
            success: function (data) {
                new SuccessAjax(data, list);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown, request, data) {
                new ErrorAjax(list);
            }
        });
    }
}
var StartLoading = function (data) {
    $(function () {
        $("#" + data.paginationid).pagination({
            items: 1,
            itemsOnPage: 10,
            cssStyle: 'list-pagination',
            onPageClick: function (pageNumber, event) {
                list.page = pageNumber;
                list.getList(list);
            },
            onInit: function () {
                new Initialization(data);
            }
        });
    });
}
var Initialization = function (data) {
    Promise.all([
        getTemplate(data.template.path),
        getTemplate("shared.html")
    ]).then(function (values) {
        data.templaterenderData = $.templates(data.template.name, values[0]);
        data.templaterender = $.templates("SharedTemplate", values[1]);
        var html = data.templaterender.render({
            "secondtemplate": data.template.id
        });
        $("#" + data.id).html(html);
        new List().getList(data);
    }).catch(function (err) {
        new List().getList(data);
        console.log(err);
    });
}

function getTemplate(relPath) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/StaticFile/GetJRenderTemplate',
            data: {
                relPath: relPath
            },
            success: function (data) {
                if (data) {
                    resolve(data);
                } else {
                    reject(new Error("TemplateData Invalid!!!"))
                }
            },
            error: function (err) {
                reject(err);
            }
        });
    });
}
var ErrorAjax = function (list) {
    list.EndTime();
    list.default.model = list.model;
    var data = list.default;
    var html = list.templaterenderData(list.default);
    $("#" + list.template.id).html(html);
    var rec = list.CalculateTime();
    $("#record").show();
    $("#rec").html(rec);
}

var SuccessAjax = function (data, list) {
    list.EndTime();
    $("#" + list.paginationid).show();
    $(".perpage").show();
    data.model = list.model;
    var html = null;
    if (data.result !== null) {
        html = list.templaterenderData(data);
        $("#" + list.template.id).html(html);
    }
    else if (!data.is_success) {
        data.result = [];
        html = list.templaterenderData(data);
        $("#" + list.template.id).html(html);
    }
    var counts = data.pagination.total_count;
    $(function () {
        $("#" + list.paginationid).pagination('updateItems', counts);
    });
    var per = list.perPage;
    $("#changePag").change(function () {
        var str = "";
        $("select option:selected").each(function () {
            str += $(this).text();
        });
        list.setPerPage(str);
        list.getList(list);
    });
    var rec = list.CalculateTime();
    $("#record").show();
    $("#rec").html(rec);
}

