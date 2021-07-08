// Example Realtime Server Script
'use strict';

// Example override configuration
const configuration = {
    pingIntervalTime: 30000,
    maxPlayers: 8
};

// Timing mechanism used to trigger end of game session. Defines how long, in milliseconds, between each tick in the example tick loop
const tickTime = 1000;

// Defines how to long to wait in Seconds before beginning early termination check in the example tick loop
const minimumElapsedTime = 120;

var session; // The Realtime server session object
var logger; // Log at appropriate level via .info(), .warn(), .error(), .debug()
var startTime; // Records the time the process started
var activePlayers = 0; // Records the number of connected players
var onProcessStartedCalled = false; // Record if onProcessStarted has been called

// Example custom op codes for user-defined messages
// Any positive op code number can be defined here. These should match your client code.
const OP_CODE_CUSTOM_OP1 = 111;
const OP_CODE_CUSTOM_OP1_REPLY = 112;
const OP_CODE_PLAYER_ACCEPTED = 113;
const OP_CODE_DISCONNECT_NOTIFICATION = 114;
const ChatSent = 1;
const MoveFront = 2;
const Moveright = 3;
const Moveleft = 4;
const Moveback = 5;
const Login = 6;
const Logout = 7;
const Change_Floor1 = 8;
const Change_Floor2 = 9;
const Change_Floor3 = 10;
const Change_Wall1 = 11;
const Change_Wall2 = 12;
const Change_Wall3 = 13;
const Change_Sex_Man = 14;
const Change_Sex_Woman = 15;
var a;

// Example groups for user-defined groups
// Any positive group number can be defined here. These should match your client code.
// When referring to user-defined groups, "-1" represents all groups, "0" is reserved.
const AllPlayer = -1;
const RED_TEAM_GROUP = 1;
const BLUE_TEAM_GROUP = 2;

// Called when game server is initialized, passed server's object of current session
function init(rtSession) {
    session = rtSession;
    logger = session.getLogger();
}

// On Process Started is called when the process has begun and we need to perform any
// bootstrapping.  This is where the developer should insert any code to prepare
// the process to be able to host a game session, for example load some settings or set state
//
// Return true if the process has been appropriately prepared and it is okay to invoke the
// GameLift ProcessReady() call.
function onProcessStarted(args) {
    onProcessStartedCalled = true;
    logger.info("Starting process with args: " + args);
    logger.info("Ready to host games...");

    return true;
}

// Called when a new game session is started on the process
function onStartGameSession(gameSession) {
    // Complete any game session set-up

    // Set up an example tick loop to perform server initiated actions
    startTime = getTimeInS();
    tickLoop();
}

// Handle process termination if the process is being terminated by GameLift
// You do not need to call ProcessEnding here
function onProcessTerminate() {
    // Perform any clean up
}

// Return true if the process is healthy
function onHealthCheck() {
    return true;
}

// On Player Connect is called when a player has passed initial validation
// Return true if player should connect, false to reject
function onPlayerConnect(connectMsg) {
    // Perform any validation needed for connectMsg.payload, connectMsg.peerId
    return true;
}

// Called when a Player is accepted into the game
function onPlayerAccepted(player) {
    // This player was accepted -- let's send them a message
    const msg = session.newTextGameMessage(OP_CODE_PLAYER_ACCEPTED, player.peerId,
        "Peer " + player.peerId + " accepted");
    session.sendReliableMessage(msg, player.peerId);
    a = player.peerId;
    activePlayers++;
}

// On Player Disconnect is called when a player has left or been forcibly terminated
// Is only called for players that actually connected to the server and not those rejected by validation
// This is called before the player is removed from the player list
function onPlayerDisconnect(peerId) {
    // send a message to each remaining player letting them know about the disconnect
    const outMessage = session.newTextGameMessage(OP_CODE_DISCONNECT_NOTIFICATION,
        session.getServerId(),
        "Peer " + peerId + " disconnected");
    session.sendReliableMessage(outMessage, peerId);
    session.getPlayers().forEach((player, playerId) => {
        if (playerId != peerId) {
            session.sendReliableMessage(outMessage, peerId);
        }
    });
    activePlayers--;
}

// Handle a message to the server
function onMessage(gameMessage) {
    logger.info("sendMessage" + gameMessage.opCode);
    const outMessage = session.newTextGameMessage(gameMessage.opCode, gameMessage.sender, gameMessage.payload);
    session.sendGroupMessage(outMessage, -1);
    // session.sendReliableMessage(gameMessage.opCode, a);
    //     switch (gameMessage.opCode) {
    //         case OP_CODE_CUSTOM_OP1:
    //             {
    //                 // do operation 1 with gameMessage.payload for example sendToGroup
    //                 const outMessage = session.newTextGameMessage(OP_CODE_CUSTOM_OP1_REPLY, gameMessage.sender, gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case ChatSent:
    //             {
    //                 const outMessage = session.newTextGameMessage(ChatSent, gameMessage.sender, gameMessage.payload);
    //                 // session.sendGroupMessage(outMessage,AllPlayer);
    //                 session.sendReliableMessage(ChatSent, a);
    //                 break;
    //             }
    //         case MoveFront:
    //             {
    //                 const outMessage = session.newTextGameMessage(MoveFront, session.getServerId(), gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 session.sendReliableMessage(MoveFront, a);
    //                 break;
    //             }
    //         case Moveback:
    //             {
    //                 const outMessage = session.newTextGameMessage(Moveback, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Moveleft:
    //             {
    //                 const outMessage = session.newTextGameMessage(Moveleft, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Moveright:
    //             {
    //                 const outMessage = session.newTextGameMessage(Moveright, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Floor1:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Floor1, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Floor2:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Floor2, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Floor3:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Floor2, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Wall1:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Wall1, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Wall2:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Wall2, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         case Change_Wall3:
    //             {
    //                 const outMessage = session.newTextGameMessage(Change_Wall3, session.getServerId().gameMessage.payload);
    //                 session.sendGroupMessage(outMessage, AllPlayer);
    //                 break;
    //             }
    //         default:
    //             break;
    //     }
    // }
}

// Return true if the send should be allowed
function onSendToPlayer(gameMessage) {
    // This example rejects any payloads containing "Reject"
    return (!gameMessage.getPayloadAsText().includes("Reject"));
}

// Return true if the send to group should be allowed
// Use gameMessage.getPayloadAsText() to get the message contents
function onSendToGroup(gameMessage) {
    return true;
}

// Return true if the player is allowed to join the group
function onPlayerJoinGroup(groupId, peerId) {
    return true;
}

// Return true if the player is allowed to leave the group
function onPlayerLeaveGroup(groupId, peerId) {
    return true;
}

// A simple tick loop example
// Checks to see if a minimum amount of time has passed before seeing if the game has ended
async function tickLoop() {
    const elapsedTime = getTimeInS() - startTime;
    logger.info("Tick... " + elapsedTime + " activePlayers: " + activePlayers);

    // In Tick loop - see if all players have left early after a minimum period of time has passed
    // Call processEnding() to terminate the process and quit
    if ((activePlayers == 0) && (elapsedTime > minimumElapsedTime)) {
        logger.info("All players disconnected. Ending game");
        const outcome = await session.processEnding();
        logger.info("Completed process ending with: " + outcome);
        process.exit(0);
    } else {
        setTimeout(tickLoop, tickTime);
    }
}

// Calculates the current time in seconds
function getTimeInS() {
    return Math.round(new Date().getTime() / 1000);
}

exports.ssExports = {
    configuration: configuration,
    init: init,
    onProcessStarted: onProcessStarted,
    onMessage: onMessage,
    onPlayerConnect: onPlayerConnect,
    onPlayerAccepted: onPlayerAccepted,
    onPlayerDisconnect: onPlayerDisconnect,
    onSendToPlayer: onSendToPlayer,
    onSendToGroup: onSendToGroup,
    onPlayerJoinGroup: onPlayerJoinGroup,
    onPlayerLeaveGroup: onPlayerLeaveGroup,
    onStartGameSession: onStartGameSession,
    onProcessTerminate: onProcessTerminate,
    onHealthCheck: onHealthCheck
};