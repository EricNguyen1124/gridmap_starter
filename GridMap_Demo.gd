extends GridMap

const DEBUG_PRINT_ROOMS = false
const DEBUG_RENDER_TRIANGLES = false
const DEBUG_RENDER_EDGES = false

var levelSizeZ = 30
var levelSizeX = 30
var numberOfRooms = 6
var percentPaths = 0.6

var roomArray = []

# Called when the node enters the scene tree for the first time.
func _ready():
	generateLevel()
	set_cell_item(Vector3(0,0,0), 7)
	set_cell_item(Vector3(levelSizeX,0,levelSizeZ), 7)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if Input.is_action_just_pressed("ui_down"):
		roomArray.clear()
		generateLevel()

func generateLevel():
	var roomsPlaced = 0
	var pointsList = []
	
	while roomsPlaced < numberOfRooms:
		var potentialRoom = Room.new()
		
		var roomCollides = true
		while (roomCollides):
			potentialRoom.setRoomPosAndSize(levelSizeX, levelSizeZ)
			roomCollides = potentialRoomCollides(potentialRoom)
		# check if room collides
		potentialRoom.id = roomsPlaced
		roomArray.append(potentialRoom)
		pointsList.append(potentialRoom.worldPos)
		roomsPlaced += 1
		
	drawRooms()
	
	# fuck this!!!
	var delaunay = Delaunay.new(Rect2(0,0,50,50))
	for points in pointsList:
		delaunay.add_point(points)
	var triangles = delaunay.triangulate()
	delaunay.remove_border_triangles(triangles)
	if DEBUG_RENDER_TRIANGLES:
		DebugDraw3D.clear_all()
		for triangle in triangles:
			if !delaunay.is_border_triangle(triangle): 
				show_triangle(triangle)
	
	var edges = []
	for triangle in triangles:
		if !edgeExists(edges, triangle.edge_ab):
			edges.append(triangle.edge_ab)
		
		if !edgeExists(edges, triangle.edge_bc):
			edges.append(triangle.edge_bc)
		
		if !edgeExists(edges, triangle.edge_ca):
			edges.append(triangle.edge_ca)
	print(edges)
	
	var chosenEdges = []
	
	for i in range(edges.size() * percentPaths):
		var randomEdge = edges.pick_random()
		edges.erase(randomEdge)
		# MAKE SURE IT IS NOT REMOVING THE LAST EDGE FOR THAT ROOM!
		chosenEdges.append(randomEdge)
	
	if DEBUG_RENDER_EDGES:
		show_edges(chosenEdges)
	
	for edge in chosenEdges:
		# MORE STRAIGHT PATHS, MORE RANDOMIZATION
		for i in range(100):
			var idk = edge.a.lerp(edge.b, i * 0.01)
			set_cell_item(Vector3(idk.x, 0, idk.y), 0)


func edgeExists(edges, edge):
	for e in edges:
		if e.equals(edge):
			return true
	return false

func show_triangle(triangle: Delaunay.Triangle):
	DebugDraw3D.draw_line(
		Vector3(triangle.a.x*2,0,triangle.a.y*2), 
		Vector3(triangle.b.x*2,0,triangle.b.y*2),
		Color(1, 1, 0),
		100)
	
	DebugDraw3D.draw_line(
		Vector3(triangle.b.x*2,0,triangle.b.y*2), 
		Vector3(triangle.c.x*2,0,triangle.c.y*2),
		Color(1, 1, 0),
		100)
	
	DebugDraw3D.draw_line(
		Vector3(triangle.c.x*2,0,triangle.c.y*2), 
		Vector3(triangle.a.x*2,0,triangle.a.y*2),
		Color(1, 1, 0),
		100)

func show_edges(edges):
	DebugDraw3D.clear_all()
	for edge in edges:
		DebugDraw3D.draw_line(
			Vector3(edge.a.x*2,0,edge.a.y*2), 
			Vector3(edge.b.x*2,0,edge.b.y*2),
			Color(1, 1, 0),
			100)

func drawRooms():
	var roomNum = 0
	clear()

	for room in roomArray:
		for i in range(room.width):
			for j in range(room.height):
				set_cell_item(Vector3(room.pos.x + i, 0, room.pos.y + j), 0)
		if DEBUG_PRINT_ROOMS:
			print(room.id)
			print(room.pos)
			print(room.width)
			print(room.height)
			print(room.worldPos)
			print("")
		roomNum += 1

func potentialRoomCollides(potentialRoom):
	for room in roomArray:
			if potentialRoom.checkRoomCollide(room):
				return true
	return false

class Room:
	var id
	var pos # top left coordinate
	var worldPos
	var width
	var height
	
	var minRoomSizeX = 3
	var maxRoomSizeX = 5
	
	var minRoomSizeZ = 3
	var maxRoomSizeZ = 5

	func setRoomPosAndSize(maxX, maxZ):
		pos = Vector2(randi_range(0, maxX), randi_range(0,maxZ))
		width = randi_range(minRoomSizeX, maxRoomSizeX)
		height = randi_range(minRoomSizeZ, maxRoomSizeZ)
		var iW = float(width-1) / 2
		var iH = float(height-1) / 2
		worldPos = Vector2(pos.x + iW, pos.y + iH)

	func checkRoomCollide(room):
		var left = pos.x
		var right = pos.x + width
		var up = pos.y
		var down = pos.y + height
		
		var left2 = room.pos.x
		var right2 = room.pos.x + room.width
		var up2 = room.pos.y
		var down2 = room.pos.y + room.height
		
		if right < left2 || left > right2 || up > down2 || down < up2:
			return false
		
		return true
