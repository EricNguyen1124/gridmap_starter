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
		edge.append(triangle.edge_ab)
		edge.append(triangle.edge_bc)
		edge.append(triangle.edge_ca)
		edges.append(edge)
	return edges;
