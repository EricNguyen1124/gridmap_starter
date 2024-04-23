extends Node

func get_triangles(points):
	var delaunay = Delaunay.new()
	for point in points:
		delaunay.add_point(point);
	var triangles = delaunay.triangulate();
	delaunay.remove_border_triangles(triangles);
	var edges = [];
	for triangle in triangles:
		var edge = [];
		edge.append(triangle.a)
		edge.append(triangle.b)
		edges.append(edge)
		edge = []
		edge.append(triangle.b)
		edge.append(triangle.c)
		edges.append(edge)
		edge = []
		edge.append(triangle.c)
		edge.append(triangle.a)
		edges.append(edge)
		edge = []
	return edges;
