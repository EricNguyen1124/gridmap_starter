extends GridMap

const DEBUG_PRINT_ROOMS = false
const DEBUG_RENDER_TRIANGLES = false
const DEBUG_RENDER_EDGES = true

var levelSizeZ = 30
var levelSizeX = 40
var numberOfRooms = 6
var percentPaths = 0.5

var roomArray = []

var pathTasks = {}
# Called when the node enters the scene tree for the first time.
func _ready():
	#generateLevel()
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
	var delaunay = Delaunay.new()
	for points in pointsList:
		delaunay.add_point(points)
	var triangles = delaunay.triangulate()
	delaunay.remove_border_triangles(triangles)
	if DEBUG_RENDER_TRIANGLES:
		DebugDraw3D.clear_all()
		for triangle in triangles:
			if !delaunay.is_border_triangle(triangle): 
				show_triangle(triangle)
	
	for triangle in triangles:
		assignEdgeToRooms(triangle.edge_ab)
		assignEdgeToRooms(triangle.edge_bc)
		assignEdgeToRooms(triangle.edge_ca)
	
	var labelChildren = get_children()
	for label in labelChildren:
		if label is Label3D:
			label.queue_free()
	
	var attempts = 0
	var visited = []
	
	while visited.size() != roomArray.size():
		if attempts > 100:
			return false
		for room in roomArray:
			for edge in room.edges:
				edge.active = true
		visited = []
		dfs(visited, findRoomWithId(0))
		visited = []
		dfsActive(visited, findRoomWithId(0))
		attempts += 1
	
	var drawnEdge = []
	for room in roomArray:
		drawText(room.to_string(), room.pos * 2)
		for edge in room.edges.filter(func(e): return e.active == true):
			if !drawnEdge.has(Vector2(room.id, edge.roomId)) && !drawnEdge.has(Vector2(edge.roomId, room.id)):
				pathTasks[[room.id, edge.roomId]] = []
				var som = randomPath(room, findRoomWithId(edge.roomId), 2)
				for coord in som:
					set_cell_item(Vector3(coord.x,0,coord.y), 0)
				drawnEdge.append(Vector2(room.id, edge.roomId))
				
	#var task_id = WorkerThreadPool.add_group_task(mtRandomPath, pathTasks.size())
	#WorkerThreadPool.wait_for_group_task_completion(task_id)
	#for result in pathTasks.values():
		#for coord in result:
			#set_cell_item(Vector3(coord.x,0,coord.y), 0)
	if DEBUG_RENDER_EDGES:
		show_edges()

enum CellState {OPEN, FORCED, BLOCKED}
	
func mtRandomPath(taskIndex):
	print(pathTasks.keys()[taskIndex])
	var fromRoom = findRoomWithId(pathTasks.keys()[taskIndex][0])
	var toRoom = findRoomWithId(pathTasks.keys()[taskIndex][1])
	pathTasks[pathTasks.keys()[taskIndex]] = randomPath(fromRoom, toRoom, 2)
	
func randomPath(fromRoom, toRoom, wiggliness = 1):
	var fromPos = fromRoom.worldPos
	var toPos = toRoom.worldPos
	
	var openCells = []
	var cellStates = {}
	for x in range(levelSizeX + 5):
		for y in range(levelSizeZ + 5):
			openCells.push_back(Vector2(x,y))
			cellStates[Vector2(x,y)] = CellState.OPEN

	openCells.erase(fromPos)
	openCells.erase(toPos)
	cellStates[fromPos] = CellState.FORCED
	cellStates[toPos] = CellState.FORCED
	
	var astar = AStarGrid2D.new()
	astar.region = Rect2i(0,0,levelSizeX+5, levelSizeZ+5)
	astar.diagonal_mode = AStarGrid2D.DIAGONAL_MODE_NEVER
	astar.update()
	
	var witness = astar(astar, fromRoom, toRoom, cellStates)

	while openCells.size() != 0:
		var openPathCells = witness.filter(func(c): return cellStates[c] == CellState.OPEN)
		
		var pathWeight = openPathCells.size() * wiggliness
		var nonPathWeight = openCells.size() - openPathCells.size()
		var totalWeight = pathWeight + nonPathWeight
		#var start = Time.get_ticks_usec()
		var randomCell
		if (randf() * totalWeight) <= pathWeight:
			randomCell = openPathCells.pick_random()
		else:
			var nonPathOpenCells = openCells.filter(func(c): return !witness.has(c))
			randomCell = nonPathOpenCells.pick_random()
		
		cellStates[randomCell] = CellState.BLOCKED
		openCells.erase(randomCell)
		
		if witness.has(randomCell):
			var newPath = astar(astar, fromRoom, toRoom, cellStates)
			if newPath.is_empty():
				cellStates[randomCell] = CellState.FORCED
			else:
				witness = newPath
		#var end = Time.get_ticks_usec()
		#var worker_time = (end-start)
		#print("Worker time: %s" % [worker_time])
	return witness

func astar(astar, from, to, cellStates):
	for x in range(levelSizeX+5):
		for y in range(levelSizeZ+5):
			var room = findRoomWithVector(Vector2(x,y))
			if (room != null && room.id != from.id && room.id != to.id) || cellStates[Vector2(x,y)] == CellState.BLOCKED:
				astar.set_point_solid(Vector2i(x,y), true)
	var point_paths = astar.get_point_path(from.worldPos, to.worldPos)
	return Array(point_paths)

func dfs(visited, room):
	if !visited.has(room):
		visited.append(room)
		for edge in room.edges:
			var toRoom = findRoomWithId(edge.roomId)
			
			if randf() > percentPaths && room.edges.filter(func(e): return e.active).size() > 1: #&& toRoom.edges.filter(func(e): return e.active).size() > 1:
				edge.active = false
				toRoom.setEdgeActive(room.id, false)
			
			dfs(visited, findRoomWithId(edge.roomId))

func dfsActive(visited, room):
	if !visited.has(room):
		visited.append(room)
		var edges = room.edges.filter(func(e): return e.active == true)
		for edge in edges:
			dfsActive(visited, findRoomWithId(edge.roomId))

func _dfs_randomlyDeactivateEdge(room, edge):
	if randf() > percentPaths && room.edges.filter(func(e): return e.active == true).size() > 1:
				edge.active = false

func assignEdgeToRooms(edge):
	var fromRoom = findRoomWithVector(edge.a)
	var toRoom = findRoomWithVector(edge.b)
	
	if !fromRoom.edges.any(func(edge): return edge.roomId == toRoom.id):
		var e = Edge.new(toRoom.id)
		fromRoom.edges.append(e)
	
	if !toRoom.edges.any(func(edge): return edge.roomId == fromRoom.id):
		var e = Edge.new(fromRoom.id)
		toRoom.edges.append(e)

func findRoomWithVector(point):
	for room in roomArray:
		if room.isPointInside(point):
			return room
			
func findRoomPosWithId(id):
	var room = findRoomWithId(id)
	return room.worldPos

func findRoomWithId(id):
	for room in roomArray:
		if room.id == id:
			return room

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

func show_edges():
	DebugDraw3D.clear_all()
	for room in roomArray:
		for edge in room.edges:
			if edge.active:
				var toRoom = findRoomWithId(edge.roomId)
				DebugDraw3D.draw_arrow(
					Vector3(room.worldPos.x*2,0,room.worldPos.y*2), 
					Vector3(toRoom.worldPos.x*2,0,toRoom.worldPos.y*2),
					Color(1, 1, 0), 0.02, false, 100)
	#for edge in edges:
		#DebugDraw3D.draw_line(
			#Vector3(edge.a.x*2,0,edge.a.y*2), 
			#Vector3(edge.b.x*2,0,edge.b.y*2),
			#Color(1, 1, 0),
			#100)

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
	
func drawText(text, pos):
		var label3d = Label3D.new()
		add_child(label3d)
		label3d.set_rotation_degrees(Vector3(-90,0,0))
		label3d.set_scale(Vector3(10,10,10))
		label3d.set_text(text)
		label3d.set_position(Vector3(pos.x, 2, pos.y))
		
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
	var edges = []
	
	var minRoomSizeX = 3
	var maxRoomSizeX = 4
	
	var minRoomSizeZ = 3
	var maxRoomSizeZ = 4

	func setRoomPosAndSize(maxX, maxZ):
		pos = Vector2(randi_range(0, maxX), randi_range(0,maxZ))
		width = randi_range(minRoomSizeX, maxRoomSizeX)
		height = randi_range(minRoomSizeZ, maxRoomSizeZ)
		var iW = float(width-1) / 2
		var iH = float(height-1) / 2
		worldPos = Vector2(pos.x + iW + 0.5, pos.y + iH + 0.5)

	func checkRoomCollide(room):
		var left = pos.x
		var right = pos.x + width
		var up = pos.y
		var down = pos.y + height
		
		var left2 = room.pos.x
		var right2 = room.pos.x + room.width
		var up2 = room.pos.y
		var down2 = room.pos.y + room.height
		
		if right + 2 < left2 || left - 2 > right2 || up - 2 > down2 || down + 2 < up2:
			return false
		
		return true
	
	func isPointInside(point):
		if point.x < pos.x || point.x > pos.x + width || point.y < pos.y || point.y > pos.y + height:
			return false
		return true
		
	func setEdgeActive(roomId, active):
		for edge in edges:
			if edge.roomId == roomId:
				edge.active = active
				return
		print("COULDN'T FIND EDGE " + str(roomId))
	
	func _to_string():
		var coords = "(%s,%s) " % [pos.x, pos.y]
		var id = "%s to " % [id]
		var edgeIds = ", ".join(edges.filter(func(e): return e.active == true).map(func(e): return e.roomId))	
		return id + edgeIds

class Edge:
	var active = true
	var roomId
	
	func _init(id):
		roomId = id
