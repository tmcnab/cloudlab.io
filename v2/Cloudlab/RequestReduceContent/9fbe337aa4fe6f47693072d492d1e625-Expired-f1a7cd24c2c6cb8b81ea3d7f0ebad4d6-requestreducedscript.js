/*!
* Note:  While Microsoft is not the author of this script file, Microsoft
* grants you the right to use this file for the sole purpose of either: 
* (i) interacting through your browser with the Microsoft website, subject 
* to the website's terms of use; or (ii) using the files as included with a
* Microsoft product subject to the Microsoft Software License Terms for that
* Microsoft product. Microsoft reserves all other rights to the files not 
* expressly granted by Microsoft, whether by implication, estoppel or
* otherwise. The notices and licenses below are for informational purposes 
* only.
*
* Provided by Informational Purposes Only
* MIT License
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense, 
* and/or sell copies of the Software, and to permit persons to whom the 
* Software is furnished to do so, subject to the following conditions:
*
* The copyright notice and this permission notice shall be included in all 
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
* DEALINGS IN THE SOFTWARE.
*
* Modernizr v2.0.6
* http://www.modernizr.com
*
* Copyright (c) 2009-2011 Faruk Ates, Paul Irish, Alex Sexton
*/
window.Modernizr=function(n,t,i){function l(n){s.cssText=n}function lt(n,t){return l(h.join(n+";")+(t||""))}function a(n,t){return typeof n===t}function b(n,t){return!!~(""+n).indexOf(t)}function k(n,t){for(var r in n)if(s[n[r]]!==i)return t=="pfx"?n[r]:!0;return!1}function o(n,t){var i=n.charAt(0).toUpperCase()+n.substr(1),r=(n+" "+c.join(i+" ")+i).split(" ");return k(r,t)}function ht(){u.input=function(n){for(var t=0,i=n.length;t<i;t++)rt[n[t]]=!!(n[t]in f);return rt}("autocomplete autofocus list placeholder max min multiple pattern required step".split(" ")),u.inputtypes=function(n){for(var o=0,r,u,s,h=n.length;o<h;o++)f.setAttribute("type",u=n[o]),r=f.type!=="text",r&&(f.value=nt,f.style.cssText="position:absolute;visibility:hidden;",/^range$/.test(u)&&f.style.WebkitAppearance!==i?(e.appendChild(f),s=t.defaultView,r=s.getComputedStyle&&s.getComputedStyle(f,null).WebkitAppearance!=="textfield"&&f.offsetHeight!==0,e.removeChild(f)):/^(search|tel)$/.test(u)||(/^(url|email)$/.test(u)?r=f.checkValidity&&f.checkValidity()===!1:/^color$/.test(u)?(e.appendChild(f),e.offsetWidth,r=f.value!=nt,e.removeChild(f)):r=f.value!=nt)),et[n[o]]=!!r;return et}("search tel url email datetime date month week time datetime-local number range color".split(" "))}var vt="2.0.6",u={},st=!0,e=t.documentElement,yt=t.head||t.getElementsByTagName("head")[0],v="modernizr",ut=t.createElement(v),s=ut.style,f=t.createElement("input"),nt=":)",ft=Object.prototype.toString,h=" -webkit- -moz- -o- -ms- -khtml- ".split(" "),c="Webkit Moz O ms Khtml".split(" "),g={svg:"http://www.w3.org/2000/svg"},r={},et={},rt={},ot=[],p,it=function(n,i,r,u){var s,h,o,f=t.createElement("div");if(parseInt(r,10))while(r--)o=t.createElement("div"),o.id=u?u[r]:v+(r+1),f.appendChild(o);return s=["&shy;","<style>",n,"</style>"].join(""),f.id=v,f.innerHTML+=s,e.appendChild(f),h=i(f,n),f.parentNode.removeChild(f),!!h},at=function(t){if(n.matchMedia)return matchMedia(t).matches;var i;return it("@media "+t+" { #"+v+" { position: absolute; } }",function(t){i=(n.getComputedStyle?getComputedStyle(t,null):t.currentStyle).position=="absolute"}),i},y=function(){function n(n,u){u=u||t.createElement(r[n]||"div"),n="on"+n;var f=n in u;return f||(u.setAttribute||(u=t.createElement("div")),u.setAttribute&&u.removeAttribute&&(u.setAttribute(n,""),f=a(u[n],"function"),a(u[n],i)||(u[n]=i),u.removeAttribute(n))),u=null,f}var r={select:"input",change:"input",submit:"form",reset:"form",error:"img",load:"img",abort:"img"};return n}(),tt={}.hasOwnProperty,w,ct,d;w=a(tt,i)||a(tt.call,i)?function(n,t){return t in n&&a(n.constructor.prototype[t],i)}:function(n,t){return tt.call(n,t)},ct=function(i,r){var e=i.join(""),f=r.length;it(e,function(i,r){for(var o=t.styleSheets[t.styleSheets.length-1],s=o.cssRules&&o.cssRules[0]?o.cssRules[0].cssText:o.cssText||"",h=i.childNodes,e={};f--;)e[h[f].id]=h[f];u.touch="ontouchstart"in n||e.touch.offsetTop===9,u.csstransforms3d=e.csstransforms3d.offsetLeft===9,u.generatedcontent=e.generatedcontent.offsetHeight>=1,u.fontface=/src/i.test(s)&&s.indexOf(r.split(" ")[0])===0},f,r)}(['@font-face {font-family:"font";src:url("https://")}',["@media (",h.join("touch-enabled),("),v,")","{#touch{top:9px;position:absolute}}"].join(""),["@media (",h.join("transform-3d),("),v,")","{#csstransforms3d{left:9px;position:absolute}}"].join(""),['#generatedcontent:after{content:"',nt,'";visibility:hidden}'].join("")],["fontface","touch","csstransforms3d","generatedcontent"]),r.flexbox=function(){function u(n,t,i,r){t+=":",n.style.cssText=(t+h.join(i+";"+t)).slice(0,-t.length)+(r||"")}function f(n,t,i,r){n.style.cssText=h.join(t+":"+i+";")+(r||"")}var n=t.createElement("div"),i=t.createElement("div"),r;return u(n,"display","box","width:42px;padding:0;"),f(i,"box-flex","1","width:10px;"),n.appendChild(i),e.appendChild(n),r=i.offsetWidth===42,n.removeChild(i),e.removeChild(n),r},r.canvas=function(){var n=t.createElement("canvas");return!!(n.getContext&&n.getContext("2d"))},r.canvastext=function(){return!!(u.canvas&&a(t.createElement("canvas").getContext("2d").fillText,"function"))},r.webgl=function(){return!!n.WebGLRenderingContext},r.touch=function(){return u.touch},r.geolocation=function(){return!!navigator.geolocation},r.postmessage=function(){return!!n.postMessage},r.websqldatabase=function(){return!!n.openDatabase},r.indexedDB=function(){for(var t=-1,i=c.length;++t<i;)if(n[c[t].toLowerCase()+"IndexedDB"])return!0;return!!n.indexedDB},r.hashchange=function(){return y("hashchange",n)&&(t.documentMode===i||t.documentMode>7)},r.history=function(){return!!(n.history&&history.pushState)},r.draganddrop=function(){return y("dragstart")&&y("drop")},r.websockets=function(){for(var t=-1,i=c.length;++t<i;)if(n[c[t]+"WebSocket"])return!0;return"WebSocket"in n},r.rgba=function(){return l("background-color:rgba(150,255,150,.5)"),b(s.backgroundColor,"rgba")},r.hsla=function(){return l("background-color:hsla(120,40%,100%,.5)"),b(s.backgroundColor,"rgba")||b(s.backgroundColor,"hsla")},r.multiplebgs=function(){return l("background:url(https://),url(https://),red url(https://)"),/(url\s*\(.*?){3}/.test(s.background)},r.backgroundsize=function(){return o("backgroundSize")},r.borderimage=function(){return o("borderImage")},r.borderradius=function(){return o("borderRadius")},r.boxshadow=function(){return o("boxShadow")},r.textshadow=function(){return t.createElement("div").style.textShadow===""},r.opacity=function(){return lt("opacity:.55"),/^0.55$/.test(s.opacity)},r.cssanimations=function(){return o("animationName")},r.csscolumns=function(){return o("columnCount")},r.cssgradients=function(){var n="background-image:",i="gradient(linear,left top,right bottom,from(#9f9),to(white));",t="linear-gradient(left top,#9f9, white);";return l((n+h.join(i+n)+h.join(t+n)).slice(0,-n.length)),b(s.backgroundImage,"gradient")},r.cssreflections=function(){return o("boxReflect")},r.csstransforms=function(){return!!k(["transformProperty","WebkitTransform","MozTransform","OTransform","msTransform"])},r.csstransforms3d=function(){var n=!!k(["perspectiveProperty","WebkitPerspective","MozPerspective","OPerspective","msPerspective"]);return n&&"webkitPerspective"in e.style&&(n=u.csstransforms3d),n},r.csstransitions=function(){return o("transitionProperty")},r.fontface=function(){return u.fontface},r.generatedcontent=function(){return u.generatedcontent},r.video=function(){var i=t.createElement("video"),n=!1,r;try{(n=!!i.canPlayType)&&(n=new Boolean(n),n.ogg=i.canPlayType('video/ogg; codecs="theora"'),r='video/mp4; codecs="avc1.42E01E',n.h264=i.canPlayType(r+'"')||i.canPlayType(r+', mp4a.40.2"'),n.webm=i.canPlayType('video/webm; codecs="vp8, vorbis"'))}catch(u){}return n},r.audio=function(){var i=t.createElement("audio"),n=!1;try{(n=!!i.canPlayType)&&(n=new Boolean(n),n.ogg=i.canPlayType('audio/ogg; codecs="vorbis"'),n.mp3=i.canPlayType("audio/mpeg;"),n.wav=i.canPlayType('audio/wav; codecs="1"'),n.m4a=i.canPlayType("audio/x-m4a;")||i.canPlayType("audio/aac;"))}catch(r){}return n},r.localstorage=function(){try{return!!localStorage.getItem}catch(n){return!1}},r.sessionstorage=function(){try{return!!sessionStorage.getItem}catch(n){return!1}},r.webworkers=function(){return!!n.Worker},r.applicationcache=function(){return!!n.applicationCache},r.svg=function(){return!!t.createElementNS&&!!t.createElementNS(g.svg,"svg").createSVGRect},r.inlinesvg=function(){var n=t.createElement("div");return n.innerHTML="<svg/>",(n.firstChild&&n.firstChild.namespaceURI)==g.svg},r.smil=function(){return!!t.createElementNS&&/SVG/.test(ft.call(t.createElementNS(g.svg,"animate")))},r.svgclippaths=function(){return!!t.createElementNS&&/SVG/.test(ft.call(t.createElementNS(g.svg,"clipPath")))};for(d in r)w(r,d)&&(p=d.toLowerCase(),u[p]=r[d](),ot.push((u[p]?"":"no-")+p));return u.input||ht(),u.addTest=function(n,t){if(typeof n=="object")for(var r in n)w(n,r)&&u.addTest(r,n[r]);else{if(n=n.toLowerCase(),u[n]!==i)return;t=typeof t=="boolean"?t:!!t(),e.className+=" "+(t?"":"no-")+n,u[n]=t}return u},l(""),ut=f=null,n.attachEvent&&function(){var n=t.createElement("div");return n.innerHTML="<elem></elem>",n.childNodes.length!==1}()&&function(n,t){function v(n){for(var t=-1;++t<a;)n.createElement(h[t])}n.iepp=n.iepp||{};var r=n.iepp,s=r.html5elements||"abbr|article|aside|audio|canvas|datalist|details|figcaption|figure|footer|header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",h=s.split("|"),a=h.length,k=new RegExp("(^|\\s)("+s+")","gi"),b=new RegExp("<(/*)("+s+")","gi"),p=/^\s*[\{\}]\s*$/,w=new RegExp("(^|[^\\n]*?\\s)("+s+")([^\\n]*)({[\\n\\w\\W]*?})","gi"),l=t.createDocumentFragment(),o=t.documentElement,c=o.firstChild,f=t.createElement("body"),e=t.createElement("style"),y=/print|all/,u;(r.getCSS=function(n,t){if(n+""===i)return"";for(var e=-1,o=n.length,u,f=[];++e<o;)(u=n[e],u.disabled)||(t=u.media||t,y.test(t)&&f.push(r.getCSS(u.imports,t),u.cssText),t="all");return f.join("")},r.parseCSS=function(n){for(var i=[],t;(t=w.exec(n))!=null;)i.push(((p.exec(t[1])?"\n":t[1])+t[2]+t[3]).replace(k,"$1.iepp_$2")+t[4]);return i.join("\n")},r.writeHTML=function(){var r=-1;for(u=u||t.body;++r<a;)for(var i=t.getElementsByTagName(h[r]),e=i.length,n=-1;++n<e;)i[n].className.indexOf("iepp_")<0&&(i[n].className+=" iepp_"+h[r]);l.appendChild(u),o.appendChild(f),f.className=u.className,f.id=u.id,f.innerHTML=u.innerHTML.replace(b,"<$1font")},r._beforePrint=function(){e.styleSheet.cssText=r.parseCSS(r.getCSS(t.styleSheets,"all")),r.writeHTML()},r.restoreHTML=function(){f.innerHTML="",o.removeChild(f),o.appendChild(u)},r._afterPrint=function(){r.restoreHTML(),e.styleSheet.cssText=""},v(t),v(l),r.disablePP)||(c.insertBefore(e,c.firstChild),e.media="print",e.className="iepp-printshim",n.attachEvent("onbeforeprint",r._beforePrint),n.attachEvent("onafterprint",r._afterPrint))}(n,t),u._version=vt,u._prefixes=h,u._domPrefixes=c,u.mq=at,u.hasEvent=y,u.testProp=function(n){return k([n])},u.testAllProps=o,u.testStyles=it,u.prefixed=function(n){return o(n,"pfx")},e.className=e.className.replace(/\bno-js\b/,"")+(st?" js "+ot.join(" "):""),u}(this,this.document)