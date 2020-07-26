

'use strict';

var ServerDate = (function(serverNow) {


var
  scriptLoadTime = Date.now(),

  scripts = document.getElementsByTagName("script"),
  URL = scripts[scripts.length - 1].src,

  synchronizationIntervalDelay,
  synchronizationInterval,
  precision,
  offset,
  target = null,
  synchronizing = false;

function ServerDate() {
  return this
    ? ServerDate
    : ServerDate.toString();
}

ServerDate.parse = Date.parse;
ServerDate.UTC = Date.UTC;

ServerDate.now = function() {
  return Date.now() + offset;
};

["toString", "toDateString", "toTimeString", "toLocaleString",
  "toLocaleDateString", "toLocaleTimeString", "valueOf", "getTime",
  "getFullYear", "getUTCFullYear", "getMonth", "getUTCMonth", "getDate",
  "getUTCDate", "getDay", "getUTCDay", "getHours", "getUTCHours",
  "getMinutes", "getUTCMinutes", "getSeconds", "getUTCSeconds",
  "getMilliseconds", "getUTCMilliseconds", "getTimezoneOffset", "toUTCString",
  "toISOString", "toJSON"]
  .forEach(function(method) {
    ServerDate[method] = function() {
      return new Date(ServerDate.now())[method]();
    };
  });

ServerDate.getPrecision = function() // ms
{
  if (typeof target.precision != "undefined")
    // Take into account the amortization.
    return target.precision + Math.abs(target - offset);
};

ServerDate.amortizationRate = 25; 

ServerDate.amortizationThreshold = 2000; 

Object.defineProperty(ServerDate, "synchronizationIntervalDelay", {
  get: function() { return synchronizationIntervalDelay; },

  set: function(value) {
  synchronizationIntervalDelay = value;
  clearInterval(synchronizationInterval);

  synchronizationInterval = setInterval(synchronize,
    ServerDate.synchronizationIntervalDelay);

  log("Set synchronizationIntervalDelay to " + value + " ms.");
}});

ServerDate.synchronizationIntervalDelay = 60 *20* 1000; // ms

function Offset(value, precision) {
  this.value = value;
  this.precision = precision;
}

Offset.prototype.valueOf = function() {
  return this.value;
};

Offset.prototype.toString = function() {

  return this.value + (typeof this.precision != "undefined"
    ? " +/- " + this.precision
    : "") + " ms";
};

function setTarget(newTarget) {
  var message = "Set target to " + String(newTarget),
    delta;

  if (target)
    message += " (" + (newTarget > target ? "+" : "-") + " "
      + Math.abs(newTarget - target) + " ms)";

  target = newTarget;

  delta = Math.abs(target - offset);

  if (delta > ServerDate.amortizationThreshold) {
    

    offset = target;
  }
}

function synchronize() {
  var iteration = 1,
    requestTime, responseTime,
    best;

  function requestSample() {
    var request = new XMLHttpRequest();

    request.open("GET", URL + "?time=now");

    request.onreadystatechange = function() {
      if ((this.readyState == this.HEADERS_RECEIVED)
        && (this.status == 200))
        responseTime = Date.now();
    };

    request.onload = function() {
      if (this.status == 200) {
        try {
          processSample(JSON.parse(this.response));
        }
        catch (exception) {
          
        }
      }
    };

    requestTime = Date.now();

    request.send();
  }

  function processSample(serverNow) {
    var precision = (responseTime - requestTime) / 2,
      sample = new Offset(serverNow + precision - responseTime,
        precision);

    if ((iteration == 1) || (precision <= best.precision))
      best = sample;

    if (iteration < 10) {
      iteration++;
      requestSample();
    }
    else {
      setTarget(best);

      synchronizing = false;
    }
  }

  if (!synchronizing) {
    synchronizing = true;

    setTimeout(function () {
      synchronizing = false;
    },
    10 * 1000);

    requestSample();
  }
}

function log(message) {
}

offset = serverNow - scriptLoadTime;

if (typeof performance != "undefined") {
  precision = (scriptLoadTime - performance.timing.domLoading) / 2;
  offset += precision;
}

setTarget(new Offset(offset, precision));

setInterval(function()
{
  var delta = Math.max(-ServerDate.amortizationRate,
    Math.min(ServerDate.amortizationRate, target - offset));

  offset += delta;

  
}, 1000);

window.addEventListener('pageshow', synchronize);

synchronize();

return ServerDate;
})(1593246513530);