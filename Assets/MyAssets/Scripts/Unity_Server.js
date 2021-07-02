'use strict';

var session;
var logger;

function init(rtSession) {}

function onProcessStarted(args) {
    return true;
}

function onStartGameSession(gameSession) {}

function onProcessTerminate() {}

function onPlayerConnect(connectMsg) {
    return true;
}

function onPlayerAccepted(player) {}

function onPlayerDisconnect(peerId) {}

function onPlayerJoinGroup(groupId, peerId) {
    return true;
}

function onPlayerLeaveGroup(groupId, peerId) {
    return true;
}

function onSendToPlayer(gameMessage) {
    return true;
}

function onSendToGroup(gameMessage) {
    return true;
}

function onMessage(gameMessage) {}

function onHealthCheck() {
    return true;
}

exports.ssExports = {
    init: init,
    onProcessStarted: onProcessStarted,
    onStartGameSession: onStartGameSession,
    onProcessTerminate: onProcessTerminate,
    onPlayerConnect: onPlayerConnect,
    onPlayerAccepted: onPlayerAccepted,
    onPlayerDisconnect: onPlayerDisconnect,
    onPlayerJoinGroup: onPlayerJoinGroup,
    onPlayerLeaveGroup: onPlayerLeaveGroup,
    onSendToPlayer: onSendToPlayer,
    onSendToGroup: onSendToGroup,
    onMessage: onMessage,
    onHealthCheck: onHealthCheck
};