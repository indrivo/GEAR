$(function () {
    "use strict";
    $(function () {
        $(".preloader").fadeOut()
    }), jQuery(document).on("click", ".mega-dropdown", function (i) {
        i.stopPropagation()
    });
    var i = function () {
        var i = window.innerWidth > 0 ? window.innerWidth : this.screen.width,
            e = 70;
        1170 > i ? ($("body").addClass("mini-sidebar"), $(".navbar-brand span").hide(), $(".scroll-sidebar, .slimScrollDiv").css("overflow-x", "visible").parent().css("overflow", "visible"), $(".sidebartoggler i").addClass("ti-menu")) : ($("body").removeClass("mini-sidebar"), $(".navbar-brand span").show(), $(".sidebartoggler i").removeClass("ti-menu"));
        var s = (window.innerHeight > 0 ? window.innerHeight : this.screen.height) - 1;
        s -= e, 1 > s && (s = 1), s > e && $(".page-wrapper").css("min-height", s + "px")
    };
    $(window).ready(i), $(window).on("resize", i), $(".sidebartoggler").on("click",
        function () {
            $("body").hasClass("mini-sidebar")
                ? ($("body").trigger("resize"),
                    $(".scroll-sidebar, .slimScrollDiv").css("overflow", "hidden").parent()
                        .css("overflow", "visible"), $("body").removeClass("mini-sidebar"),
                    $(".navbar-brand span").show(), $(".sidebartoggler i").addClass("ti-menu"))
                : ($("body").trigger("resize"), $(".scroll-sidebar, .slimScrollDiv").css("overflow-x", "visible")
                    .parent()
                    .css("overflow", "visible"), $("body").addClass("mini-sidebar"), $(".navbar-brand span")
                        .hide(),
                    $(".sidebartoggler i").removeClass("ti-menu"))
        }), $(".fix-header .topbar").stick_in_parent({}), $(".nav-toggler").click(function () {
            $("body").toggleClass("show-sidebar"), $(".nav-toggler i").toggleClass("ti-menu"), $(".nav-toggler i")
                .addClass("ti-close")
        }), $(".sidebartoggler").on("click",
            function () {
                $(".sidebartoggler i").toggleClass("ti-menu")
            }), $(".right-side-toggle").click(function () {
                $(".right-sidebar-central").slideDown(50), $(".right-sidebar-central").toggleClass("shw-rside")
            }), $(function () {
                for (var i = window.location,
                    e = $("ul#sidebarnav a").filter(function () {
                        return this.href == i
                    }).addClass("active").parent().addClass("active"); ;
                ) {
                    if (!e.is("li")) break;
                    e = e.parent().addClass("in").parent().addClass("active")
                }
            }), $(function () {
                $('[data-toggle="tooltip"]').tooltip()
            }), $(function () {
                $('[data-toggle="popover"]').popover()
            }), $(function () {
                $("#sidebarnav").metisMenu()
            }), $(".scroll-sidebar").slimScroll({
                position: "left",
                size: "5px",
                height: "100%",
                color: "#dcdcdc"
            }), $(".message-center").slimScroll({
                position: "right",
                size: "5px",
                color: "#dcdcdc"
            }), $(".aboutscroll").slimScroll({
                position: "right",
                size: "5px",
                height: "80",
                color: "#dcdcdc"
            }), $(".message-scroll").slimScroll({
                position: "right",
                size: "5px",
                height: "570",
                color: "#dcdcdc"
            }), $(".chat-box").slimScroll({
                position: "right",
                size: "5px",
                height: "470",
                color: "#dcdcdc"
            }), $(".slimscrollright").slimScroll({
                height: "100%",
                position: "right",
                size: "5px",
                color: "#dcdcdc"
            }), $("body").trigger("resize"), $(".list-task li label").click(function () {
                $(this).toggleClass("task-done")
            }), $("#to-recover").on("click",
                function () {
                    $("#loginform").slideUp(), $("#recoverform").fadeIn()
                }), $(document).on("click",
                    ".card-actions a",
                    function (i) {
                        i.preventDefault(), $(this).hasClass("btn-close") && $(this).parent().parent().parent().fadeOut()
                    }),
        function (i, e, s) {
            var o = '[data-perform="card-collapse"]';
            i(o).each(function () {
                var e = i(this),
                    s = e.closest(".card"),
                    o = s.find(".card-block"),
                    r = {
                        toggle: !1
                    };
                o.length ||
                    (o = s.children(".card-heading").nextAll().wrapAll("<div/>").parent().addClass("card-block"), r =
                        {}), o.collapse(r).on("hide.bs.collapse",
                            function () {
                                e.children("i").removeClass("ti-minus").addClass("ti-plus")
                            }).on("show.bs.collapse",
                                function () {
                                    e.children("i").removeClass("ti-plus").addClass("ti-minus")
                                })
            }), i(s).on("click",
                o,
                function (e) {
                    e.preventDefault();
                    var s = i(this).closest(".card"),
                        o = s.find(".card-block");
                    o.collapse("toggle")
                })
        }(jQuery, window, document);

    (function (global, factory) {
        if (typeof define === "function" && define.amd) {
            define(['jquery'], factory);
        } else if (typeof exports !== "undefined") {
            factory(require('jquery'));
        } else {
            var mod = {
                exports: {}
            };
            factory(global.jquery);
            global.metisMenu = mod.exports;
        }
    })(this, function (_jquery) {
        'use strict';

        var _jquery2 = _interopRequireDefault(_jquery);

        function _interopRequireDefault(obj) {
            return obj && obj.__esModule ? obj : {
                default: obj
            };
        }

        var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
            return typeof obj;
        } : function (obj) {
            return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
        };

        function _classCallCheck(instance, Constructor) {
            if (!(instance instanceof Constructor)) {
                throw new TypeError("Cannot call a class as a function");
            }
        }

        var Util = function ($) {
            var transition = false;

            var TransitionEndEvent = {
                WebkitTransition: 'webkitTransitionEnd',
                MozTransition: 'transitionend',
                OTransition: 'oTransitionEnd otransitionend',
                transition: 'transitionend'
            };

            function getSpecialTransitionEndEvent() {
                return {
                    bindType: transition.end,
                    delegateType: transition.end,
                    handle: function handle(event) {
                        if ($(event.target).is(this)) {
                            return event.handleObj.handler.apply(this, arguments);
                        }
                        return undefined;
                    }
                };
            }

            function transitionEndTest() {
                if (window.QUnit) {
                    return false;
                }

                var el = document.createElement('mm');

                for (var name in TransitionEndEvent) {
                    if (el.style[name] !== undefined) {
                        return {
                            end: TransitionEndEvent[name]
                        };
                    }
                }

                return false;
            }

            function transitionEndEmulator(duration) {
                var _this2 = this;

                var called = false;

                $(this).one(Util.TRANSITION_END, function () {
                    called = true;
                });

                setTimeout(function () {
                    if (!called) {
                        Util.triggerTransitionEnd(_this2);
                    }
                }, duration);

                return this;
            }

            function setTransitionEndSupport() {
                transition = transitionEndTest();
                $.fn.emulateTransitionEnd = transitionEndEmulator;

                if (Util.supportsTransitionEnd()) {
                    $.event.special[Util.TRANSITION_END] = getSpecialTransitionEndEvent();
                }
            }

            var Util = {
                TRANSITION_END: 'mmTransitionEnd',

                triggerTransitionEnd: function triggerTransitionEnd(element) {
                    $(element).trigger(transition.end);
                },
                supportsTransitionEnd: function supportsTransitionEnd() {
                    return Boolean(transition);
                }
            };

            setTransitionEndSupport();

            return Util;
        }(jQuery);

        var MetisMenu = function ($) {
            var NAME = 'metisMenu';
            var DATA_KEY = 'metisMenu';
            var EVENT_KEY = '.' + DATA_KEY;
            var DATA_API_KEY = '.data-api';
            var JQUERY_NO_CONFLICT = $.fn[NAME];
            var TRANSITION_DURATION = 350;

            var Default = {
                toggle: true,
                preventDefault: true,
                activeClass: 'active',
                collapseClass: 'collapse',
                collapseInClass: 'in',
                collapsingClass: 'collapsing',
                triggerElement: 'a',
                parentTrigger: 'li',
                subMenu: 'ul'
            };

            var Event = {
                SHOW: 'show' + EVENT_KEY,
                SHOWN: 'shown' + EVENT_KEY,
                HIDE: 'hide' + EVENT_KEY,
                HIDDEN: 'hidden' + EVENT_KEY,
                CLICK_DATA_API: 'click' + EVENT_KEY + DATA_API_KEY
            };

            var MetisMenu = function () {
                function MetisMenu(element, config) {
                    _classCallCheck(this, MetisMenu);

                    this._element = element;
                    this._config = this._getConfig(config);
                    this._transitioning = null;

                    this.init();
                }

                MetisMenu.prototype.init = function init() {
                    var self = this;
                    $(this._element).find(this._config.parentTrigger + '.' + this._config.activeClass).has(this._config.subMenu).children(this._config.subMenu).attr('aria-expanded', true).addClass(this._config.collapseClass + ' ' + this._config.collapseInClass);

                    $(this._element).find(this._config.parentTrigger).not('.' + this._config.activeClass).has(this._config.subMenu).children(this._config.subMenu).attr('aria-expanded', false).addClass(this._config.collapseClass);

                    $(this._element).find(this._config.parentTrigger).has(this._config.subMenu).children(this._config.triggerElement).on(Event.CLICK_DATA_API, function (e) {
                        var _this = $(this);
                        var _parent = _this.parent(self._config.parentTrigger);
                        var _siblings = _parent.siblings(self._config.parentTrigger).children(self._config.triggerElement);
                        var _list = _parent.children(self._config.subMenu);
                        if (self._config.preventDefault) {
                            e.preventDefault();
                        }
                        if (_this.attr('aria-disabled') === 'true') {
                            return;
                        }
                        if (_parent.hasClass(self._config.activeClass)) {
                            _this.attr('aria-expanded', false);
                            self._hide(_list);
                        } else {
                            self._show(_list);
                            _this.attr('aria-expanded', true);
                            if (self._config.toggle) {
                                _siblings.attr('aria-expanded', false);
                            }
                        }

                        if (self._config.onTransitionStart) {
                            self._config.onTransitionStart(e);
                        }
                    });
                };

                MetisMenu.prototype._show = function _show(element) {
                    if (this._transitioning || $(element).hasClass(this._config.collapsingClass)) {
                        return;
                    }
                    var _this = this;
                    var _el = $(element);

                    var startEvent = $.Event(Event.SHOW);
                    _el.trigger(startEvent);

                    if (startEvent.isDefaultPrevented()) {
                        return;
                    }

                    _el.parent(this._config.parentTrigger).addClass(this._config.activeClass);

                    if (this._config.toggle) {
                        this._hide(_el.parent(this._config.parentTrigger).siblings().children(this._config.subMenu + '.' + this._config.collapseInClass).attr('aria-expanded', false));
                    }

                    _el.removeClass(this._config.collapseClass).addClass(this._config.collapsingClass).height(0);

                    this.setTransitioning(true);

                    var complete = function complete() {
                        _el.removeClass(_this._config.collapsingClass).addClass(_this._config.collapseClass + ' ' + _this._config.collapseInClass).height('').attr('aria-expanded', true);

                        _this.setTransitioning(false);

                        _el.trigger(Event.SHOWN);
                    };

                    if (!Util.supportsTransitionEnd()) {
                        complete();
                        return;
                    }

                    _el.height(_el[0].scrollHeight).one(Util.TRANSITION_END, complete).emulateTransitionEnd(TRANSITION_DURATION);
                };

                MetisMenu.prototype._hide = function _hide(element) {
                    if (this._transitioning || !$(element).hasClass(this._config.collapseInClass)) {
                        return;
                    }
                    var _this = this;
                    var _el = $(element);

                    var startEvent = $.Event(Event.HIDE);
                    _el.trigger(startEvent);

                    if (startEvent.isDefaultPrevented()) {
                        return;
                    }

                    _el.parent(this._config.parentTrigger).removeClass(this._config.activeClass);
                    _el.height(_el.height())[0].offsetHeight;

                    _el.addClass(this._config.collapsingClass).removeClass(this._config.collapseClass).removeClass(this._config.collapseInClass);

                    this.setTransitioning(true);

                    var complete = function complete() {
                        if (_this._transitioning && _this._config.onTransitionEnd) {
                            _this._config.onTransitionEnd();
                        }

                        _this.setTransitioning(false);
                        _el.trigger(Event.HIDDEN);

                        _el.removeClass(_this._config.collapsingClass).addClass(_this._config.collapseClass).attr('aria-expanded', false);
                    };

                    if (!Util.supportsTransitionEnd()) {
                        complete();
                        return;
                    }

                    _el.height() == 0 || _el.css('display') == 'none' ? complete() : _el.height(0).one(Util.TRANSITION_END, complete).emulateTransitionEnd(TRANSITION_DURATION);
                };

                MetisMenu.prototype.setTransitioning = function setTransitioning(isTransitioning) {
                    this._transitioning = isTransitioning;
                };

                MetisMenu.prototype.dispose = function dispose() {
                    $.removeData(this._element, DATA_KEY);

                    $(this._element).find(this._config.parentTrigger).has(this._config.subMenu).children(this._config.triggerElement).off('click');

                    this._transitioning = null;
                    this._config = null;
                    this._element = null;
                };

                MetisMenu.prototype._getConfig = function _getConfig(config) {
                    config = $.extend({}, Default, config);
                    return config;
                };

                MetisMenu._jQueryInterface = function _jQueryInterface(config) {
                    return this.each(function () {
                        var $this = $(this);
                        var data = $this.data(DATA_KEY);
                        var _config = $.extend({}, Default, $this.data(), (typeof config === 'undefined' ? 'undefined' : _typeof(config)) === 'object' && config);

                        if (!data && /dispose/.test(config)) {
                            this.dispose();
                        }

                        if (!data) {
                            data = new MetisMenu(this, _config);
                            $this.data(DATA_KEY, data);
                        }

                        if (typeof config === 'string') {
                            if (data[config] === undefined) {
                                throw new Error('No method named "' + config + '"');
                            }
                            data[config]();
                        }
                    });
                };

                return MetisMenu;
            }();

			/**
			 * ------------------------------------------------------------------------
			 * jQuery
			 * ------------------------------------------------------------------------
			 */

            $.fn[NAME] = MetisMenu._jQueryInterface;
            $.fn[NAME].Constructor = MetisMenu;
            $.fn[NAME].noConflict = function () {
                $.fn[NAME] = JQUERY_NO_CONFLICT;
                return MetisMenu._jQueryInterface;
            };
            return MetisMenu;
        }(jQuery);
    });

    !function (t) { "use strict"; function e(t) { return null !== t && t === t.window } function n(t) { return e(t) ? t : 9 === t.nodeType && t.defaultView } function a(t) { var e, a, i = { top: 0, left: 0 }, o = t && t.ownerDocument; return e = o.documentElement, "undefined" != typeof t.getBoundingClientRect && (i = t.getBoundingClientRect()), a = n(o), { top: i.top + a.pageYOffset - e.clientTop, left: i.left + a.pageXOffset - e.clientLeft } } function i(t) { var e = ""; for (var n in t) t.hasOwnProperty(n) && (e += n + ":" + t[n] + ";"); return e } function o(t) { if (d.allowEvent(t) === !1) return null; for (var e = null, n = t.target || t.srcElement; null !== n.parentElement;) { if (!(n instanceof SVGElement || -1 === n.className.indexOf("waves-effect"))) { e = n; break } if (n.classlist.contains("waves-effect")) { e = n; break } n = n.parentElement } return e } function r(e) { var n = o(e); null !== n && (c.show(e, n), "ontouchstart" in t && (n.addEventListener("touchend", c.hide, !1), n.addEventListener("touchcancel", c.hide, !1)), n.addEventListener("mouseup", c.hide, !1), n.addEventListener("mouseleave", c.hide, !1)) } var s = s || {}, u = document.querySelectorAll.bind(document), c = { duration: 750, show: function (t, e) { if (2 === t.button) return !1; var n = e || this, o = document.createElement("div"); o.className = "waves-ripple", n.appendChild(o); var r = a(n), s = t.pageY - r.top, u = t.pageX - r.left, d = "scale(" + n.clientWidth / 100 * 10 + ")"; "touches" in t && (s = t.touches[0].pageY - r.top, u = t.touches[0].pageX - r.left), o.setAttribute("data-hold", Date.now()), o.setAttribute("data-scale", d), o.setAttribute("data-x", u), o.setAttribute("data-y", s); var l = { top: s + "px", left: u + "px" }; o.className = o.className + " waves-notransition", o.setAttribute("style", i(l)), o.className = o.className.replace("waves-notransition", ""), l["-webkit-transform"] = d, l["-moz-transform"] = d, l["-ms-transform"] = d, l["-o-transform"] = d, l.transform = d, l.opacity = "1", l["-webkit-transition-duration"] = c.duration + "ms", l["-moz-transition-duration"] = c.duration + "ms", l["-o-transition-duration"] = c.duration + "ms", l["transition-duration"] = c.duration + "ms", l["-webkit-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["-moz-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["-o-transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", l["transition-timing-function"] = "cubic-bezier(0.250, 0.460, 0.450, 0.940)", o.setAttribute("style", i(l)) }, hide: function (t) { d.touchup(t); var e = this, n = (1.4 * e.clientWidth, null), a = e.getElementsByClassName("waves-ripple"); if (!(a.length > 0)) return !1; n = a[a.length - 1]; var o = n.getAttribute("data-x"), r = n.getAttribute("data-y"), s = n.getAttribute("data-scale"), u = Date.now() - Number(n.getAttribute("data-hold")), l = 350 - u; 0 > l && (l = 0), setTimeout(function () { var t = { top: r + "px", left: o + "px", opacity: "0", "-webkit-transition-duration": c.duration + "ms", "-moz-transition-duration": c.duration + "ms", "-o-transition-duration": c.duration + "ms", "transition-duration": c.duration + "ms", "-webkit-transform": s, "-moz-transform": s, "-ms-transform": s, "-o-transform": s, transform: s }; n.setAttribute("style", i(t)), setTimeout(function () { try { e.removeChild(n) } catch (t) { return !1 } }, c.duration) }, l) }, wrapInput: function (t) { for (var e = 0; e < t.length; e++) { var n = t[e]; if ("input" === n.tagName.toLowerCase()) { var a = n.parentNode; if ("i" === a.tagName.toLowerCase() && -1 !== a.className.indexOf("waves-effect")) continue; var i = document.createElement("i"); i.className = n.className + " waves-input-wrapper"; var o = n.getAttribute("style"); o || (o = ""), i.setAttribute("style", o), n.className = "waves-button-input", n.removeAttribute("style"), a.replaceChild(i, n), i.appendChild(n) } } } }, d = { touches: 0, allowEvent: function (t) { var e = !0; return "touchstart" === t.type ? d.touches += 1 : "touchend" === t.type || "touchcancel" === t.type ? setTimeout(function () { d.touches > 0 && (d.touches -= 1) }, 500) : "mousedown" === t.type && d.touches > 0 && (e = !1), e }, touchup: function (t) { d.allowEvent(t) } }; s.displayEffect = function (e) { e = e || {}, "duration" in e && (c.duration = e.duration), c.wrapInput(u(".waves-effect")), "ontouchstart" in t && document.body.addEventListener("touchstart", r, !1), document.body.addEventListener("mousedown", r, !1) }, s.attach = function (e) { "input" === e.tagName.toLowerCase() && (c.wrapInput([e]), e = e.parentElement), "ontouchstart" in t && e.addEventListener("touchstart", r, !1), e.addEventListener("mousedown", r, !1) }, t.Waves = s, document.addEventListener("DOMContentLoaded", function () { s.displayEffect() }, !1) }(window);
});