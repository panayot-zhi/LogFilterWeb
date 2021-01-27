// Panayot Ivanov

var chartColors = {
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