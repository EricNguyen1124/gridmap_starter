extends GridMap

var levelSizeZ = 12
var levelSizeX = 12
var numberOfRooms = 10

var roomArray = []

# Called when the node enters the scene tree for the first time.
func _ready():
	set_cell_item(Vector3(0,0,0), 7)
	set_cell_item(Vector3(levelSizeX,0,levelSizeZ), 7)
	generateLevel(roomArray)
	
	var roomNum = 0
	for room in roomArray:
		print(roomNum)
		print(room.pos)
		print(room.width)
		print(room.height)
		for i in range(room.width):
			for j in range(room.height):
				set_cell_item(Vector3(room.pos.x + i, 0, room.pos.y + j), 0)
		print("")
		roomNum += 1

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func generateLevel(rooms):
	var roomsPlaced = 0
	while roomsPlaced < numberOfRooms:
		var potentialRoom = Room.new()
		potentialRoom.setRoomPos(levelSizeX, levelSizeZ)
		# check if room collides
		rooms.append(potentialRoom)
		roomsPlaced += 1


class Room:
	var pos # top right coordinate
	var width
	var height
	
	var minRoomSizeX = 2
	var maxRoomSizeX = 3
	
	var minRoomSizeZ = 2
	var maxRoomSizeZ = 3

	func setRoomPos(maxX, maxZ):
		pos = Vector2(randi_range(0, maxX), randi_range(0,maxZ))
		setRoomSize()

	func setRoomSize():
		width = randi_range(minRoomSizeX, maxRoomSizeX)
		height = randi_range(minRoomSizeZ, maxRoomSizeZ)

	func checkRoomCollide(room):
		room.pos
		return true
