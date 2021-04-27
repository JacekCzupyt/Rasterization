# Rasterization

This is an application with a graphical user interface for creating and editing simple vector graphics: lines, polygons and circles.

## Implementation

Lines and circles are drawn using the midpoint line algorithm and antialiased using the Xiaolin Wu algorithm.
Thick lines are drawn using the brush algorithm.

## How to use

### Lines

Click the line button, then left click on the canvas to begin drawing. 

Use mousewheel to change thickness, using the minimum value will cause it to use the thin-line midpoint algorithm instead of the thick line brush algorithm.

Left click again to finish drawing, or right click to cancel.

### Polygons

Click the polygon button, then left click on the canvas to begin drawing. 

Change thickness in the same way as lines.

Left click again to add a vertex, right click (or left click the starting point) to finish drawing.

### Circles

Click the circle button, then left click to pick the circle center. 

Left click again to pick the radius, or right click to cancel.

Circles do not have width.

### Capsules

Click the capsule button, then left click to pick the first point.

Left click again to choose the second point, and then again to choose a size.

Right click to cancel drawing. Capsules do not have width.

### Selecting and moving points

When no shape button is pressed, left click to select a point. 

Ctrl + left click to select or deselect multiple points. 

Doubble click to select all points belonging to a shape.

Left click and drag (anywhere on the canvas) to move all selected points.

Use the mouse wheel to change the thickness of the shapes that the selected points belong to.

Press delete in order to remove the shapes that the selected points belong to.

Right click to deselect all points.

### Colors

Pick a color before drawing a shape in order to set its color.

Pick a new color while some points are selected in order to recolor the shapes they belong to.

### Menu

Use the menu to turn antialiesing on or off, clear the image, save it, export it to jpg or png, or load an existing image.

