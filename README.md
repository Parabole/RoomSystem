# RoomSystem

## Components

### RoomNetwork

Network of rooms linked together by portals.

### Room

Defines a room the player can get into. Its visibility is determined by its connection to other rooms through portals.

### RoomPortal

Connects room together and determine which rooms can be seen from other rooms. Can be closed to follow the state of certain elements of the environment, such as doors. By default, the player can see one portal away. There are ways, such as See Through rooms, to make visibility go through 2 or more portal by chaining them. Chains of portals will need every portal it contains to be open to be considered open itself.

### RoomContent

Content linked to the state of the room(s) it is assigned to. Used for the actual effect of the state of its room(s) (Active, Visible, Standby)

## Editor Tools

### Room Network Authoring Inspector

![Room Network Authoring Interface](/Documentation/RoomNetworkAuthoringInterface.png)

### Room Authoring Inspector

![Room Authoring Interface](/Documentation/RoomAuthoringInterface.png)

*See Through:* Room that allows adjacent rooms to be visible from each other by looking through it. It will cause every portals on this room to be chained together.

### Door Portal Link Menu

![Door Portal Link Menu](/Documentation/DoorPortalLinkTools.png)