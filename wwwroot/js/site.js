// Panayot Ivanov

const availableColors = ["#3366cc", "#dc3912", "#ff9900", "#109618", "#990099", "#0099c6", "#dd4477", "#66aa00", "#b82e2e", "#316395", "#3366cc", "#994499", "#22aa99", "#aaaa11", "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac", "#b77322", "#16d620", "#b91383", "#f4359e", "#9c5935", "#a9c413", "#2a778d", "#668d1c", "#bea413", "#0c5922", "#743411"];

// TODO: Color shenanigans, remove

const chartColors = {
  white: "#FFFFFF",
  darkblue: "#1e3d60",
  blue: "rgb(54, 162, 235)",
  green: "rgb(75, 192, 192)",
  grey: "rgb(201, 203, 207)",
  orange: "rgb(255, 159, 64)",
  purple: "rgb(153, 102, 255)",
  red: "rgb(255, 99, 132)",
  yellow: "rgb(255, 205, 86)"
}

function hex(c) {
    var s = "0123456789abcdef";
    var i = parseInt(c);
    if (i == 0 || isNaN(c))
        return "00";
    i = Math.round(Math.min(Math.max(0, i), 255));
    return s.charAt((i - i % 16) / 16) + s.charAt(i % 16);
}

/* Convert an RGB triplet to a hex string */
function convertToHex(rgb) {
    return hex(rgb[0]) + hex(rgb[1]) + hex(rgb[2]);
}

/* Remove '#' in color hex string */
function trim(s) { return (s.charAt(0) == '#') ? s.substring(1, 7) : s }

/* Convert a hex string to an RGB triplet */
function convertToRGB(hex) {
    var color = [];
    color[0] = parseInt((trim(hex)).substring(0, 2), 16);
    color[1] = parseInt((trim(hex)).substring(2, 4), 16);
    color[2] = parseInt((trim(hex)).substring(4, 6), 16);
    return color;
}

function generateColors(colorStart, colorEnd, colorCount) {

    // The beginning of your gradient
    var start = convertToRGB(colorStart);

    // The end of your gradient
    var end = convertToRGB(colorEnd);

    // The number of colors to compute
    var len = colorCount;

    //Alpha blending amount
    var alpha = 0.0;

    var colors = [];

    for (i = 0; i < len; i++) {
        var c = [];
        alpha += (1.0 / len);

        c[0] = start[0] * alpha + (1 - alpha) * end[0];
        c[1] = start[1] * alpha + (1 - alpha) * end[1];
        c[2] = start[2] * alpha + (1 - alpha) * end[2];

        colors.push("#" + convertToHex(c));

    }

    return colors;

}

function drawBlueCanvasBackground(ctx) {

    const gradientStroke = ctx.createLinearGradient(500, 0, 100, 0);
    gradientStroke.addColorStop(0, "#80b6f4");
    gradientStroke.addColorStop(1, "#FFFFFF");

    const gradientFill = ctx.createLinearGradient(0, 200, 0, 50);
    gradientFill.addColorStop(0, "rgba(128, 182, 244, 0)");
    gradientFill.addColorStop(1, "rgba(255, 255, 255, 0.24)");

    return gradientFill;
}

function drawRedCanvasBackground(ctx) {

    const gradientStroke = ctx.createLinearGradient(500, 0, 100, 0);
    gradientStroke.addColorStop(0, "#80b6f4");
    gradientStroke.addColorStop(1, "#FFFFFF");

    const gradientFill = ctx.createLinearGradient(0, 170, 0, 50);
    gradientFill.addColorStop(0, "rgba(128, 182, 244, 0)");
    gradientFill.addColorStop(1, "rgba(249, 99, 59, 0.40)");

    return gradientFill;
}

function drawGreenCanvasBackground(ctx) {

    const gradientStroke = ctx.createLinearGradient(500, 0, 100, 0);
    gradientStroke.addColorStop(0, "#18ce0f");
    gradientStroke.addColorStop(1, "#FFFFFF");

    const gradientFill = ctx.createLinearGradient(0, 170, 0, 50);
    gradientFill.addColorStop(0, "rgba(128, 182, 244, 0)");
    gradientFill.addColorStop(1, hexToRGB("#18ce0f", 0.4));

    return gradientFill;
}

function showNotification(text) {
    $.notify({
        message: text

    }, {
        type: "primary",
        timer: 8000,
        placement: {
            from: "top",
            align: "right"
        }
    });
}

function showErrorNotification(text) {
    $.notify({
        icon: "now-ui-icons ui-1_bell-53",
        message: "<b>Възникна грешка:</b> " + text

    }, {
        type: "danger",
        timer: 8000,
        placement: {
            from: "top",
            align: "right"
        }
    });
}

function showWarnNotification(text) {
    $.notify({
        icon: "now-ui-icons ui-1_bell-53",
        message: text

    }, {
        type: "warning",
        timer: 8000,
        placement: {
            from: "top",
            align: "right"
        }
    });
}

function showInfoNotification(text) {
    $.notify({
        icon: "now-ui-icons travel_info",
        message: text

    }, {
        type: "info",
        timer: 8000,
        placement: {
            from: "top",
            align: "right"
        }
    });
}

function showSuccessNotification(text) {
    $.notify({
        icon: "now-ui-icons emoticons_satisfied",
        message: text

    }, {
        type: "success",
        timer: 8000,
        placement: {
            from: "top",
            align: "right"
        }
    });
}

function nFormatter(num, digits) {
  var si = [
    { value: 1, symbol: "" },
    { value: 1E3, symbol: "k" },
    { value: 1E6, symbol: "M" },
    { value: 1E9, symbol: "G" },
    { value: 1E12, symbol: "T" },
    { value: 1E15, symbol: "P" },
    { value: 1E18, symbol: "E" }
  ];
  var rx = /\.0+$|(\.[0-9]*[1-9])0+$/;
  var i;
  for (i = si.length - 1; i > 0; i--) {
    if (num >= si[i].value) {
      break;
    }
  }
  return (num / si[i].value).toFixed(digits).replace(rx, "$1") + si[i].symbol;
}

function kFormatter(num) {
  return Math.abs(num) > 999
    ? Math.sign(num) * ((Math.abs(num) / 1000).toFixed(1)) + "k"
    : Math.sign(num) * Math.abs(num);
}

function bytesToSize(bytes) {
    const sizes = ["Bytes", "KB", "MB", "GB", "TB"];
    if (bytes == 0) return "0 Byte";
    let i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return (bytes / Math.pow(1024, i)).toFixed(2) + " " + sizes[i];
}

function cutOffAt(message, length) {
    if (!length) {
        length = 304;
    }

    if (message.length < length) {
        return message;
    }

    return message.substring(0, length - 4) + " ...";
}

function updateQueryStringParameter(uri, key, value) {
    const re = new RegExp(`([?&])${key}=.*?(&|$)`, "i");
    let separator = uri.indexOf("?") !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, `$1${key}=${value}$2`);
    }
    else {
        return uri + separator + key + "=" + value;
    }
}

$(document).ajaxError(function globalErrorHandler(event, xhr, ajaxOptions, thrownError) {
    showErrorNotification(`${ajaxOptions.type} ${ajaxOptions.url} ${xhr.status} <b>${xhr.statusText.toLocaleUpperCase()}</b><br><br>${cutOffAt(xhr.responseText)}`, "danger");
});