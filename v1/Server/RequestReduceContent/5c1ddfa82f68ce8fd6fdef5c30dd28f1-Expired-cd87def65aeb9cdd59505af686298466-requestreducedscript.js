$(function(){"use strict";$.support.transition=function(){var i=document.body||document.documentElement,n=i.style,t=n.transition!==undefined||n.WebkitTransition!==undefined||n.MozTransition!==undefined||n.MsTransition!==undefined||n.OTransition!==undefined;return t&&{end:function(){var n="TransitionEnd";return $.browser.webkit?n="webkitTransitionEnd":$.browser.mozilla?n="transitionend":$.browser.opera&&(n="oTransitionEnd"),n}()}}()}),!function(n){"use strict";function e(){var t=this,r=setTimeout(function(){t.$element.off(n.support.transition.end),i.call(t)},500);this.$element.one(n.support.transition.end,function(){clearTimeout(r),i.call(t)})}function i(){this.$element.hide().trigger("hidden"),f.call(this)}function f(t){var f=this,u=this.$element.hasClass("fade")?"fade":"",i;this.isShown&&this.options.backdrop?(i=n.support.transition&&u,this.$backdrop=n('<div class="modal-backdrop '+u+'" />').appendTo(document.body),this.options.backdrop!="static"&&this.$backdrop.click(n.proxy(this.hide,this)),i&&this.$backdrop[0].offsetWidth,this.$backdrop.addClass("in"),i?this.$backdrop.one(n.support.transition.end,t):t()):!this.isShown&&this.$backdrop?(this.$backdrop.removeClass("in"),n.support.transition&&this.$element.hasClass("fade")?this.$backdrop.one(n.support.transition.end,n.proxy(r,this)):r.call(this)):t&&t()}function r(){this.$backdrop.remove(),this.$backdrop=null}function u(){var t=this;if(this.isShown&&this.options.keyboard)n(document).on("keyup.dismiss.modal",function(n){n.which==27&&t.hide()});else this.isShown||n(document).off("keyup.dismiss.modal")}var t=function(t,i){this.options=n.extend({},n.fn.modal.defaults,i),this.$element=n(t).delegate('[data-dismiss="modal"]',"click.dismiss.modal",n.proxy(this.hide,this))};t.prototype={constructor:t,toggle:function(){return this[this.isShown?"hide":"show"]()},show:function(){var t=this;if(this.isShown)return;this.isShown=!0,this.$element.trigger("show"),u.call(this),f.call(this,function(){var i=n.support.transition&&t.$element.hasClass("fade");!t.$element.parent().length&&t.$element.appendTo(document.body),t.$element.show(),i&&t.$element[0].offsetWidth,t.$element.addClass("in"),i?t.$element.one(n.support.transition.end,function(){t.$element.trigger("shown")}):t.$element.trigger("shown")})},hide:function(t){t&&t.preventDefault();if(!this.isShown)return;var r=this;this.isShown=!1,u.call(this),this.$element.trigger("hide").removeClass("in"),n.support.transition&&this.$element.hasClass("fade")?e.call(this):i.call(this)}},n.fn.modal=function(i){return this.each(function(){var u=n(this),r=u.data("modal"),f=typeof i=="object"&&i;r||u.data("modal",r=new t(this,f)),typeof i=="string"?r[i]():r.show()})},n.fn.modal.defaults={backdrop:!0,keyboard:!0},n.fn.modal.Constructor=t,n(function(){n("body").on("click.modal.data-api",'[data-toggle="modal"]',function(t){var r=n(this),i=n(r.attr("data-target")||r.attr("href")),u=i.data("modal")?"toggle":n.extend({},i.data(),r.data());t.preventDefault(),i.modal(u)})})}(window.jQuery),!function(n){"use strict";function i(){n(r).parent().removeClass("open")}var r='[data-toggle="dropdown"]',t=function(t){n(t).bind("click",this.toggle)};t.prototype={constructor:t,toggle:function(){var u=n(this),e=u.attr("data-target")||u.attr("href"),r=n(e),f;return r.length||(r=u.parent()),f=r.hasClass("open"),i(),!f&&r.toggleClass("open"),!1}},n.fn.dropdown=function(i){return this.each(function(){var u=n(this),r=u.data("dropdown");r||u.data("dropdown",r=new t(this)),typeof i=="string"&&r[i].call(u)})},n.fn.dropdown.Constructor=t,n(function(){n(window).on("click.dropdown.data-api",i);n("body").on("click.dropdown.data-api",r,t.prototype.toggle)})}(window.jQuery),!function(n){"use strict";function t(t){var i=n.proxy(this.process,this);this.$scrollElement=n(t).on("scroll.scroll.data-api",i),this.selector=(this.$scrollElement.attr("data-target")||this.$scrollElement.attr("href")||"")+" .nav li > a",this.$body=n("body").on("click.scroll.data-api",this.selector,i),this.refresh(),this.process()}t.prototype={constructor:t,refresh:function(){this.targets=this.$body.find(this.selector).map(function(){var t=n(this).attr("href");return/^#\w/.test(t)&&n(t).length?t:null}),this.offsets=n.map(this.targets,function(t){return n(t).position().top})},process:function(){for(var i=this.$scrollElement.scrollTop()+10,t=this.offsets,r=this.targets,u=this.activeTarget,n=t.length;n--;)u!=r[n]&&i>=t[n]&&(!t[n+1]||i<=t[n+1])&&this.activate(r[n])},activate:function(n){var t;this.activeTarget=n,this.$body.find(this.selector).parent(".active").removeClass("active"),t=this.$body.find(this.selector+'[href="'+n+'"]').parent("li").addClass("active"),t.parent(".dropdown-menu")&&t.closest("li.dropdown").addClass("active")}},n.fn.scrollspy=function(i){return this.each(function(){var u=n(this),r=u.data("scrollspy");r||u.data("scrollspy",r=new t(this)),typeof i=="string"&&r[i]()})},n.fn.scrollspy.Constructor=t,n(function(){n('[data-spy="scroll"]').scrollspy()})}(window.jQuery),!function(n){"use strict";var t=function(t){this.element=n(t)};t.prototype={constructor:t,show:function(){var t=this.element,u=t.closest("ul:not(.dropdown-menu)"),f=t.attr("data-target")||t.attr("href"),i,r;if(t.parent("li").hasClass("active"))return;i=u.find(".active a").last()[0],t.trigger({type:"show",relatedTarget:i}),r=n(f),this.activate(t.parent("li"),u),this.activate(r,r.parent(),function(){t.trigger({type:"shown",relatedTarget:i})})},activate:function(t,i,r){function f(){u.removeClass("active").find("> .dropdown-menu > .active").removeClass("active"),t.addClass("active"),e?(t[0].offsetWidth,t.addClass("in")):t.removeClass("fade"),t.parent(".dropdown-menu")&&t.closest("li.dropdown").addClass("active"),r&&r()}var u=i.find("> .active"),e=r&&n.support.transition&&u.hasClass("fade");e?u.one(n.support.transition.end,f):f(),u.removeClass("in")}},n.fn.tab=function(i){return this.each(function(){var u=n(this),r=u.data("tab");r||u.data("tab",r=new t(this)),typeof i=="string"&&r[i]()})},n.fn.tab.Constructor=t,n(function(){n("body").on("click.tab.data-api",'[data-toggle="tab"], [data-toggle="pill"]',function(t){t.preventDefault(),n(this).tab("show")})})}(window.jQuery),!function(n){"use strict";var t=function(n,t){this.init("tooltip",n,t)};t.prototype={constructor:t,init:function(t,i,r){var f,u;this.type=t,this.$element=n(i),this.options=this.getOptions(r),this.enabled=!0;if(this.options.trigger!="manual"){f=this.options.trigger=="hover"?"mouseenter":"focus",u=this.options.trigger=="hover"?"mouseleave":"blur";this.$element.on(f,this.options.selector,n.proxy(this.enter,this));this.$element.on(u,this.options.selector,n.proxy(this.leave,this))}this.options.selector?this._options=n.extend({},this.options,{trigger:"manual",selector:""}):this.fixTitle()},getOptions:function(t){return t=n.extend({},n.fn[this.type].defaults,t,this.$element.data()),t.delay&&typeof t.delay=="number"&&(t.delay={show:t.delay,hide:t.delay}),t},enter:function(t){var i=n(t.currentTarget)[this.type](this._options).data(this.type);i.options.delay&&i.options.delay.show?(i.hoverState="in",setTimeout(function(){i.hoverState=="in"&&i.show()},i.options.delay.show)):i.show()},leave:function(t){var i=n(t.currentTarget)[this.type](this._options).data(this.type);i.options.delay&&i.options.delay.hide?(i.hoverState="out",setTimeout(function(){i.hoverState=="out"&&i.hide()},i.options.delay.hide)):i.hide()},show:function(){var t,f,n,u,e,i,r;if(this.hasContent()&&this.enabled){t=this.tip(),this.setContent(),this.options.animation&&t.addClass("fade"),i=typeof this.options.placement=="function"?thing.call(this,t[0],this.$element[0]):this.options.placement,f=/in/.test(i),t.remove().css({top:0,left:0,display:"block"}).appendTo(f?this.$element:document.body),n=this.getPosition(f),u=t[0].offsetWidth,e=t[0].offsetHeight;switch(f?i.split(" ")[1]:i){case"bottom":r={top:n.top+n.height,left:n.left+n.width/2-u/2};break;case"top":r={top:n.top-e,left:n.left+n.width/2-u/2};break;case"left":r={top:n.top+n.height/2-e/2,left:n.left-u};break;case"right":r={top:n.top+n.height/2-e/2,left:n.left+n.width}}t.css(r).addClass(i).addClass("in")}},setContent:function(){var n=this.tip();n.find(".tooltip-inner").html(this.getTitle()),n.removeClass("fade in top bottom left right")},hide:function(){function i(){var i=setTimeout(function(){t.off(n.support.transition.end).remove()},500);t.one(n.support.transition.end,function(){clearTimeout(i),t.remove()})}var r=this,t=this.tip();t.removeClass("in"),n.support.transition&&this.$tip.hasClass("fade")?i():t.remove()},fixTitle:function(){var n=this.$element;(n.attr("title")||typeof n.attr("data-original-title")!="string")&&n.attr("data-original-title",n.attr("title")||"").removeAttr("title")},hasContent:function(){return this.getTitle()},getPosition:function(t){return n.extend({},t?{top:0,left:0}:this.$element.offset(),{width:this.$element[0].offsetWidth,height:this.$element[0].offsetHeight})},getTitle:function(){var n,i=this.$element,t=this.options;return n=i.attr("data-original-title")||(typeof t.title=="function"?t.title.call(i[0]):t.title),n=n.toString().replace(/(^\s*|\s*$)/,"")},tip:function(){return this.$tip=this.$tip||n(this.options.template)},validate:function(){this.$element[0].parentNode||(this.hide(),this.$element=null,this.options=null)},enable:function(){this.enabled=!0},disable:function(){this.enabled=!1},toggleEnabled:function(){this.enabled=!this.enabled},toggle:function(){this[this.tip().hasClass("in")?"hide":"show"]()}},n.fn.tooltip=function(i){return this.each(function(){var u=n(this),r=u.data("tooltip"),f=typeof i=="object"&&i;r||u.data("tooltip",r=new t(this,f)),typeof i=="string"&&r[i]()})},n.fn.tooltip.Constructor=t,n.fn.tooltip.defaults={animation:!0,delay:0,selector:!1,placement:"top",trigger:"hover",title:"",template:'<div class="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'}}(window.jQuery),!function(n){"use strict";var t=function(n,t){this.init("popover",n,t)};t.prototype=n.extend({},n.fn.tooltip.Constructor.prototype,{constructor:t,setContent:function(){var t=this.tip(),r=this.getTitle(),i=this.getContent();t.find(".title")[n.type(r)=="object"?"append":"html"](r),t.find(".content > *")[n.type(i)=="object"?"append":"html"](i),t[0].className="popover"},hasContent:function(){return this.getTitle()||this.getContent()},getContent:function(){var n,i=this.$element,t=this.options;return n=i.attr("data-content")||(typeof t.content=="function"?t.content.call(i[0]):t.content),n=n.toString().replace(/(^\s*|\s*$)/,"")},tip:function(){return this.$tip||(this.$tip=n(this.options.template)),this.$tip}}),n.fn.popover=function(i){return this.each(function(){var u=n(this),r=u.data("popover"),f=typeof i=="object"&&i;r||u.data("popover",r=new t(this,f)),typeof i=="string"&&r[i]()})},n.fn.popover.Constructor=t,n.fn.popover.defaults=n.extend({},n.fn.tooltip.defaults,{placement:"right",content:"",template:'<div class="popover"><div class="arrow"></div><div class="inner"><h3 class="title"></h3><div class="content"><p></p></div></div></div>'})}(window.jQuery),!function(n){"use strict";var i='[data-dismiss="alert"]',t=function(t){n(t).on("click",i,this.close)};t.prototype={constructor:t,close:function(t){function u(){i.remove(),i.trigger("closed")}var r=n(this),f=r.attr("data-target")||r.attr("href"),i=n(f);i.trigger("close"),t&&t.preventDefault(),i.length||(i=r.hasClass("alert-message")?r:r.parent()),i.removeClass("in"),n.support.transition&&i.hasClass("fade")?i.on(n.support.transition.end,u):u()}},n.fn.alert=function(i){return this.each(function(){var u=n(this),r=u.data("alert");r||u.data("alert",r=new t(this)),typeof i=="string"&&r[i].call(u)})},n.fn.alert.Constructor=t,n(function(){n("body").on("click.alert.data-api",i,t.prototype.close)})}(window.jQuery),!function(n){"use strict";var t=function(t,i){this.$element=n(t),this.options=n.extend({},n.fn.button.defaults,i)};t.prototype={constructor:t,setState:function(n){var i="disabled",t=this.$element,u=t.data(),r=t.is("input")?"val":"html";n=n+"Text",u.resetText||t.data("resetText",t[r]()),t[r](u[n]||this.options[n]),setTimeout(function(){n=="loadingText"?t.addClass(i).attr(i,i):t.removeClass(i).removeAttr(i)},0)},toggle:function(){var n=this.$element.parent('[data-toggle="buttons-radio"]');n&&n.find(".active").removeClass("active"),this.$element.toggleClass("active")}},n.fn.button=function(i){return this.each(function(){var u=n(this),r=u.data("button"),f=typeof i=="object"&&i;r||u.data("button",r=new t(this,f)),i=="toggle"?r.toggle():i&&r.setState(i)})},n.fn.button.defaults={loadingText:"loading..."},n.fn.button.Constructor=t,n(function(){n("body").on("click.button.data-api","[data-toggle^=button]",function(t){n(t.target).button("toggle")})})}(window.jQuery),!function(n){"use strict";var t=function(t,i){this.$element=n(t),this.options=n.extend({},n.fn.collapse.defaults,i),this.options.parent&&(this.$parent=n(this.options.parent)),this.options.toggle&&this.toggle()};t.prototype={constructor:t,dimension:function(){var n=this.$element.hasClass("width");return n?"width":"height"},show:function(){var i=this.dimension(),u=n.camelCase(["scroll",i].join("-")),t=this.$parent&&this.$parent.find(".in"),r;t&&t.length&&(r=t.data("collapse"),t.collapse("hide"),r||t.data("collapse",null)),this.$element[i](0),this.transition("addClass","show","shown"),this.$element[i](this.$element[0][u])},hide:function(){var n=this.dimension();this.reset(this.$element[n]()),this.transition("removeClass","hide","hidden"),this.$element[n](0)},reset:function(n){var t=this.dimension();this.$element.removeClass("collapse")[t](n||"")[0].offsetWidth,this.$element.addClass("collapse")},transition:function(t,i,r){var f=this,u=function(){i=="show"&&f.reset(),f.$element.trigger(r)};this.$element.trigger(i)[t]("in"),n.support.transition&&this.$element.hasClass("collapse")?this.$element.one(n.support.transition.end,u):u()},toggle:function(){this[this.$element.hasClass("in")?"hide":"show"]()}},n.fn.collapse=function(i){return this.each(function(){var u=n(this),r=u.data("collapse"),f=typeof i=="object"&&i;r||u.data("collapse",r=new t(this,f)),typeof i=="string"&&r[i]()})},n.fn.collapse.defaults={toggle:!0},n.fn.collapse.Constructor=t,n(function(){n("body").on("click.collapse.data-api","[data-toggle=collapse]",function(t){var i=n(this),r=i.attr("data-target")||i.attr("href"),u=n(r).data("collapse")?"toggle":i.data();t.preventDefault(),n(r).collapse(u)})})}(window.jQuery),!function(n){"use strict";var t=function(t,i){this.$element=n(t),this.options=n.extend({},n.fn.carousel.defaults,i),this.options.slide&&this.slide(this.options.slide)};t.prototype={cycle:function(){return this.interval=setInterval(n.proxy(this.next,this),this.options.interval),this},pause:function(){return clearInterval(this.interval),this},next:function(){if(!this.sliding)return this.slide("next")},prev:function(){if(!this.sliding)return this.slide("prev")},slide:function(t){var u=this.$element.find(".active"),i=u[t](),e=this.interval,r=t=="next"?"left":"right",o=t=="next"?"first":"last",f=this;this.sliding=!0,e&&this.pause(),i=i.length?i:this.$element.find(".item")[o]();if(!n.support.transition&&this.$element.hasClass("slide"))this.$element.trigger("slide"),u.removeClass("active"),i.addClass("active"),this.$element.trigger("slid"),this.sliding=!1;else{i.addClass(t),i[0].offsetWidth,u.addClass(r),i.addClass(r),this.$element.trigger("slide");this.$element.one(n.support.transition.end,function(){i.removeClass([t,r].join(" ")).addClass("active"),u.removeClass(["active",r].join(" ")),f.$element.trigger("slid"),f.sliding=!1})}return e&&this.cycle(),this}},n.fn.carousel=function(i){return this.each(function(){var f=n(this),r=f.data("carousel"),u=typeof i=="object"&&i;r||f.data("carousel",r=new t(this,u)),typeof i=="string"||(i=u.slide)?r[i]():r.cycle()})},n.fn.carousel.defaults={interval:5e3},n.fn.carousel.Constructor=t,n(function(){n("body").on("click.carousel.data-api","[data-slide]",function(){var r=n(this),i=n(r.attr("data-target")||r.attr("href")),u=!i.data("modal")&&n.extend({},i.data(),r.data());i.carousel(u)})})}(window.jQuery),!function(n){"use strict";var t=function(t,i){this.$element=n(t),this.options=n.extend({},n.fn.typeahead.defaults,i),this.$menu=n(this.options.menu).appendTo("body"),this.data=this.options.data,this.shown=!1,this.listen()};t.prototype={constructor:t,matcher:function(n,t){return~n.indexOf(t)},select:function(){var n=this.$menu.find(".active").attr("data-value");return this.$element.val(n),this.hide()},show:function(){var t=n.extend({},this.$element.offset(),{height:this.$element[0].offsetHeight});return this.$menu.css({top:t.top+t.height,left:t.left}),this.$menu.show(),this.shown=!0,this},hide:function(){return this.$menu.hide(),this.shown=!1,this},lookup:function(){var i=this.$element.val(),r=this,t;if(!i)return this.shown?this.hide():this;return t=this.data.filter(function(n){if(r.matcher(n,i))return n}),t.length?this.render(t.slice(0,this.options.items)).show():this.shown?this.hide():this},render:function(t){var i=this;return t=n(t).map(function(t,r){return t=n(i.options.item).attr("data-value",r),t.find("a").text(r),t[0]}),t.first().addClass("active"),this.$menu.html(t),this},next:function(){var r=this.$menu.find(".active").removeClass("active"),i=r.next();i.length||(i=n(this.$menu.find("li")[0])),i.addClass("active")},prev:function(){var i=this.$menu.find(".active").removeClass("active"),t=i.prev();t.length||(t=this.$menu.find("li").last()),t.addClass("active")},keyup:function(n){n.stopPropagation(),n.preventDefault();switch(n.keyCode){case 40:case 38:break;case 9:case 13:this.select();break;case 27:this.hide();break;default:this.lookup()}},keypress:function(n){n.stopPropagation();switch(n.keyCode){case 9:case 13:case 27:n.preventDefault();break;case 38:if(!this.shown)return;n.preventDefault(),this.prev();break;case 40:if(!this.shown)return;n.preventDefault(),this.next()}},blur:function(n){var t=this;n.stopPropagation(),n.preventDefault(),setTimeout(function(){t.hide()},150)},click:function(n){n.stopPropagation(),n.preventDefault(),this.select()},mouseenter:function(t){this.$menu.find(".active").removeClass("active"),n(t.currentTarget).addClass("active")},listen:function(){this.$element.on("blur",n.proxy(this.blur,this)).on("keypress",n.proxy(this.keypress,this)).on("keyup",n.proxy(this.keyup,this));if(n.browser.webkit||n.browser.msie)this.$element.on("keydown",n.proxy(this.keypress,this));this.$menu.on("click",n.proxy(this.click,this)).on("mouseenter","li",n.proxy(this.mouseenter,this))}},n.fn.typeahead=function(i){return this.each(function(){var u=n(this),r=u.data("typeahead"),f=typeof i=="object"&&i;r||u.data("typeahead",r=new t(this,f)),typeof i=="string"&&r[i]()})},n.fn.typeahead.defaults={items:8,menu:'<ul class="typeahead dropdown-menu"></ul>',item:'<li><a href="#"></a></li>'},n.fn.typeahead.Constructor=t,n(function(){n("body").on("focus.typeahead.data-api",'[data-provide="typeahead"]',function(t){var i=n(this);if(i.data("typeahead"))return;t.preventDefault(),i.typeahead(i.data())})})}(window.jQuery);var _gaq=_gaq||[];_gaq.push(["_setAccount","UA-27831838-1"]),_gaq.push(["_setDomainName","cloudlab.io"]),_gaq.push(["_trackPageview"]),function(){var n=document.createElement("script"),t;n.type="text/javascript",n.async=!0,n.src=("https:"==document.location.protocol?"https://ssl":"http://www")+".google-analytics.com/ga.js",t=document.getElementsByTagName("script")[0],t.parentNode.insertBefore(n,t)}()