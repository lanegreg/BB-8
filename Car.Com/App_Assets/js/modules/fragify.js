/*!
 * fragify.js v1.0.0
 * (c) 2014 Greg Lane - All Rights Reserved.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 */

;window._HTML = (function () {
  'use strict';

  var openTags = []
    , closeTags = [];

  var stylify = function (obj) {
    var styles = '';

    for (styleName in obj) {
      styles += styleName + ':' + (styleName === 'content' ? '"' : '') + obj[styleName] + (styleName === 'content' ? '"' : '') + ';';
    }

    return styles;
  };

  var serializeRules = function (obj) {
    var rules = '';

    for (var rule in obj) {
      rules += rule + '{' + stylify(obj[rule]) + '}';
    }

    return rules;
  };

  var serializeAttribs = function (attribs) {
    var attrs = '';

    for (var prop in attribs) {
      if (prop === 'style') {
        var styles = stylify(attribs[prop]);
        attrs += ' style="' + styles + '"';
      } else {
        var val = attribs[prop]
 					, wrapWithSingleQuotes = (val.indexOf('"') > -1);
        attrs += ' ' + prop + (wrapWithSingleQuotes ? "='" : '="') + val + (wrapWithSingleQuotes ? "'" : '"');
      }
    }

    return attrs;
  };

  var a = function (attribs) {
    openTags.push('<a' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</a>');
    return this;
  };

  var abbr = function (attribs) {
    openTags.push('<abbr' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</abbr>');
    return this;
  };

  var address = function (attribs) {
    openTags.push('<address' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</address>');
    return this;
  };

  var area = function (attribs) {
    openTags.push('<area' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var article = function (attribs) {
    openTags.push('<article' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</article>');
    return this;
  };

  var aside = function (attribs) {
    openTags.push('<aside' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</aside>');
    return this;
  };

  var audio = function (attribs) {
    openTags.push('<audio' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</audio>');
    return this;
  };

  var b = function (attribs) {
    openTags.push('<b' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</b>');
    return this;
  };

  var base = function (attribs) {
    openTags.push('<base' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var blockquote = function (attribs) {
    openTags.push('<blockquote' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</blockquote>');
    return this;
  };

  var body = function (attribs) {
    openTags.push('<body' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</body>');
    return this;
  };

  var br = function () {
    openTags.push('<br />');
    return this;
  };

  var button = function (attribs) {
    openTags.push('<button' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</button>');
    return this;
  };

  var canvas = function (attribs) {
    openTags.push('<canvas' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</canvas>');
    return this;
  };

  var caption = function (attribs) {
    openTags.push('<caption' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</caption>');
    return this;
  };

  var cite = function (attribs) {
    openTags.push('<cite' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</cite>');
    return this;
  };

  var code = function (attribs) {
    openTags.push('<code' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</code>');
    return this;
  };

  var command = function (attribs) {
    openTags.push('<command' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</command>');
    return this;
  };

  var col = function (attribs) {
    openTags.push('<col' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</col>');
    return this;
  };

  var colgroup = function (attribs) {
    openTags.push('<colgroup' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</colgroup>');
    return this;
  };

  var data = function (attribs) {
    openTags.push('<data' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</data>');
    return this;
  };

  var datalist = function (attribs) {
    openTags.push('<datalist' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</datalist>');
    return this;
  };

  var dd = function (attribs) {
    openTags.push('<dd' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</dd>');
    return this;
  };

  var del = function (attribs) {
    openTags.push('<del' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</del>');
    return this;
  };

  var details = function (attribs) {
    openTags.push('<details' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</details>');
    return this;
  };

  var dfn = function (attribs) {
    openTags.push('<dfn' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</dfn>');
    return this;
  };

  var div = function (attribs) {
    openTags.push('<div' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</div>');
    return this;
  };

  var dl = function (attribs) {
    openTags.push('<dl' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</dl>');
    return this;
  };

  var dt = function (attribs) {
    openTags.push('<dt' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</dt>');
    return this;
  };

  var em = function (attribs) {
    openTags.push('<em' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</em>');
    return this;
  };

  var embed = function (attribs) {
    openTags.push('<embed' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</embed>');
    return this;
  };

  var end = function () {
    var closeTag = closeTags.shift();
    openTags.push(closeTag);
    return this;
  };

  var fieldset = function (attribs) {
    openTags.push('<fieldset' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</fieldset>');
    return this;
  };

  var figcaption = function (attribs) {
    openTags.push('<figcaption' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</figcaption>');
    return this;
  };

  var figure = function (attribs) {
    openTags.push('<figure' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</figure>');
    return this;
  };

  var footer = function (attribs) {
    openTags.push('<footer' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</footer>');
    return this;
  };

  var form = function (attribs) {
    openTags.push('<form' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</form>');
    return this;
  };

  var h1 = function (attribs) {
    openTags.push('<h1' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h1>');
    return this;
  };

  var h2 = function (attribs) {
    openTags.push('<h2' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h2>');
    return this;
  };

  var h3 = function (attribs) {
    openTags.push('<h3' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h3>');
    return this;
  };

  var h4 = function (attribs) {
    openTags.push('<h4' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h4>');
    return this;
  };

  var h5 = function (attribs) {
    openTags.push('<h5' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h5>');
    return this;
  };

  var h6 = function (attribs) {
    openTags.push('<h6' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</h6>');
    return this;
  };

  var head = function (attribs) {
    openTags.push('<head' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</head>');
    return this;
  };

  var header = function (attribs) {
    openTags.push('<header' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</header>');
    return this;
  };

  var hr = function (attribs) {
    openTags.push('<hr' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</hr>');
    return this;
  };

  var html = function (attribs) {
    openTags.push('<html' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</html>');
    return this;
  };

  var i = function (attribs) {
    openTags.push('<i' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</i>');
    return this;
  };

  var iframe = function (attribs) {
    openTags.push('<iframe' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</iframe>');
    return this;
  };

  var img = function (attribs) {
    openTags.push('<img' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var input = function (attribs) {
    openTags.push('<input' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var ins = function (attribs) {
    openTags.push('<ins' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</ins>');
    return this;
  };

  var kbd = function (attribs) {
    openTags.push('<kbd' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</kbd>');
    return this;
  };

  var keygen = function (attribs) {
    openTags.push('<keygen' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</keygen>');
    return this;
  };

  var label = function (attribs) {
    openTags.push('<label' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</label>');
    return this;
  };

  var legend = function (attribs) {
    openTags.push('<legend' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</legend>');
    return this;
  };

  var li = function (attribs) {
    openTags.push('<li' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</li>');
    return this;
  };

  var link = function (attribs) {
    openTags.push('<link' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var main = function (attribs) {
    openTags.push('<main' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</main>');
    return this;
  };

  var map = function (attribs) {
    openTags.push('<map' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</map>');
    return this;
  };

  var mark = function (attribs) {
    openTags.push('<mark' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</mark>');
    return this;
  };

  var menu = function (attribs) {
    openTags.push('<menu' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</menu>');
    return this;
  };

  var meta = function (attribs) {
    openTags.push('<meta' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</meta>');
    return this;
  };

  var meter = function (attribs) {
    openTags.push('<meter' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</meter>');
    return this;
  };

  var nav = function (attribs) {
    openTags.push('<nav' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</nav>');
    return this;
  };

  var noscript = function (attribs) {
    openTags.push('<noscript' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</noscript>');
    return this;
  };

  var object = function (attribs) {
    openTags.push('<object' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</object>');
    return this;
  };

  var ol = function (attribs) {
    openTags.push('<ol' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</ol>');
    return this;
  };

  var optgroup = function (attribs) {
    openTags.push('<optgroup' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</optgroup>');
    return this;
  };

  var option = function (attribs) {
    openTags.push('<option' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</option>');
    return this;
  };

  var output = function (attribs) {
    openTags.push('<output' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</output>');
    return this;
  };

  var p = function (attribs) {
    openTags.push('<p' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</p>');
    return this;
  };

  var param = function (attribs) {
    openTags.push('<param' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var pre = function (attribs) {
    openTags.push('<pre' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</pre>');
    return this;
  };

  var progress = function (attribs) {
    openTags.push('<progress' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</progress>');
    return this;
  };

  var q = function (attribs) {
    openTags.push('<q' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</q>');
    return this;
  };

  var s = function (attribs) {
    openTags.push('<s' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</s>');
    return this;
  };

  var samp = function (attribs) {
    openTags.push('<samp' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</samp>');
    return this;
  };

  var script = function (attribs) {
    openTags.push('<script' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</script>');
    return this;
  };

  var section = function (attribs) {
    openTags.push('<section' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</section>');
    return this;
  };

  var select = function (attribs) {
    openTags.push('<select' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</select>');
    return this;
  };

  var small = function (attribs) {
    openTags.push('<small' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</small>');
    return this;
  };

  var source = function (attribs) {
    openTags.push('<source' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var span = function (attribs) {
    openTags.push('<span' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</span>');
    return this;
  };

  var strong = function (attribs) {
    openTags.push('<strong' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</strong>');
    return this;
  };

  var styleName = function (rules) {
    var id = rules.id || '';
    delete rules['id'];
    openTags.push('<style' + serializeAttribs({ 'id': id }) + '>' + serializeRules(rules) + '</style>');
    return this;
  };

  var sub = function (attribs) {
    openTags.push('<sub' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</sub>');
    return this;
  };

  var summary = function (attribs) {
    openTags.push('<summary' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</summary>');
    return this;
  };

  var sup = function (attribs) {
    openTags.push('<sup' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</sup>');
    return this;
  };

  var table = function (attribs) {
    openTags.push('<table' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</table>');
    return this;
  };

  var tbody = function (attribs) {
    openTags.push('<tbody' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</tbody>');
    return this;
  };

  var td = function (attribs) {
    openTags.push('<td' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</td>');
    return this;
  };

  var text = function (text) {
    openTags.push(text);
    return this;
  };

  var textarea = function (attribs) {
    openTags.push('<textarea' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</textarea>');
    return this;
  };

  var tfoot = function (attribs) {
    openTags.push('<tfoot' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</tfoot>');
    return this;
  };

  var th = function (attribs) {
    openTags.push('<th' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</th>');
    return this;
  };

  var thead = function (attribs) {
    openTags.push('<thead' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</thead>');
    return this;
  };

  var time = function (attribs) {
    openTags.push('<time' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</time>');
    return this;
  };

  var title = function (attribs) {
    openTags.push('<title' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</title>');
    return this;
  };

  var tr = function (attribs) {
    openTags.push('<tr' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</tr>');
    return this;
  };

  var track = function (attribs) {
    openTags.push('<track' + serializeAttribs(attribs) + ' />');
    return this;
  };

  var u = function (attribs) {
    openTags.push('<u' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</u>');
    return this;
  };

  var ul = function (attribs) {
    openTags.push('<ul' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</ul>');
    return this;
  };

  var vartag = function (attribs) {
    openTags.push('<var' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</var>');
    return this;
  };

  var video = function (attribs) {
    openTags.push('<video' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</video>');
    return this;
  };

  var wbr = function (attribs) {
    openTags.push('<wbr' + serializeAttribs(attribs) + '>');
    closeTags.unshift('</wbr>');
    return this;
  };


  var fragify = function () {
    var value = '';
    value += openTags.join('');
    value += closeTags.join('');
    reset();
    return value;
  };

  var clear = function () {
    reset();
    return this;
  };

  var reset = function () {
    openTags = [],
		closeTags = [];
  };

  return {
    a: a,
    abbr: abbr,
    address: address,
    area: area,
    article: article,
    aside: aside,
    audio: audio,
    b: b,
    base: base,
    blockquote: blockquote,
    body: body,
    br: br,
    button: button,
    canvas: canvas,
    caption: caption,
    cite: cite,
    code: code,
    col: col,
    colgroup: colgroup,
    command: command,
    data: data,
    datalist: datalist,
    dd: dd,
    del: del,
    details: details,
    dfn: dfn,
    div: div,
    dl: dl,
    dt: dt,
    em: em,
    ember: embed,
    end: end,
    fieldset: fieldset,
    figcaption: figcaption,
    figure: figure,
    footer: footer,
    form: form,
    h1: h1,
    h2: h2,
    h3: h3,
    h4: h4,
    h5: h5,
    h6: h6,
    head: head,
    header: header,
    hr: hr,
    html: html,
    i: i,
    iframe: iframe,
    img: img,
    input: input,
    ins: ins,
    kbd: kbd,
    keygen: keygen,
    label: label,
    legend: legend,
    li: li,
    link: link,
    object: object,
    ol: ol,
    optgroup: optgroup,
    option: option,
    output: output,
    main: main,
    map: map,
    mark: mark,
    menu: menu,
    meta: meta,
    meter: meter,
    nav: nav,
    noscript: noscript,
    p: p,
    param: param,
    pre: pre,
    progress: progress,
    q: q,
    s: s,
    samp: samp,
    script: script,
    section: section,
    select: select,
    small: small,
    source: source,
    span: span,
    strong: strong,
    style: styleName,
    sub: sub,
    summary: summary,
    sup: sup,
    table: table,
    tbody: tbody,
    td: td,
    text: text,
    textarea: textarea,
    tfoot: tfoot,
    th: th,
    thead: thead,
    time: time,
    title: title,
    tr: tr,
    track: track,
    u: u,
    ul: ul,
    vartag: vartag,
    video: video,
    wbr: wbr,

    clear: clear,
    fragify: fragify
  };
})();