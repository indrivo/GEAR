var dragSrcEl = null;

function handleDragStart(e) {
	// Target (this) element is the source node.
	dragSrcEl = this;

	e.dataTransfer.effectAllowed = 'move';
	e.dataTransfer.setData('text/html', this.outerHTML);

    $(this).addClass("dragElem");
	$(this).addClass("active");
}
function handleDragOver(e) {
	if (e.preventDefault) {
		e.preventDefault(); // Necessary. Allows us to drop.
    }
    $(this).addClass("over");

	e.dataTransfer.dropEffect = 'move';  // See the section on the DataTransfer object.
	$(this).addClass("active");
	return false;
}

function handleDragEnter(e) {
	// this / e.target is the current hover target.
}

function handleDragLeave(e) {
    $(this).removeClass("over");  // this / e.target is previous target element.
	$(this).removeClass("active");
}

function handleDrop(e) {
	// this/e.target is current target element.

	if (e.stopPropagation) {
		e.stopPropagation(); // Stops some browsers from redirecting.
	}

	// Don't do anything if dropping the same column we're dragging.
	if (dragSrcEl != this) {
		// Set the source column's HTML to the HTML of the column we dropped on.
		//alert(this.outerHTML);
		//dragSrcEl.innerHTML = this.innerHTML;
		//this.innerHTML = e.dataTransfer.getData('text/html');
		this.parentNode.removeChild(dragSrcEl);
		var dropHTML = e.dataTransfer.getData('text/html');
		this.insertAdjacentHTML('beforebegin', dropHTML);
		var dropElem = this.previousSibling;
		addDnDHandlers(dropElem);
	}
    $(this).removeClass("over");
	$(this).removeClass("active");
	return false;
}

function handleDragEnd(e) {
    // this/e.target is the source node.
    $(this).removeClass("over");
	refreshOrderItems("#columns");
}

function addDnDHandlers(elem) {
	elem.addEventListener('dragstart', handleDragStart, false);
	elem.addEventListener('dragenter', handleDragEnter, false)
	elem.addEventListener('dragover', handleDragOver, false);
	elem.addEventListener('dragleave', handleDragLeave, false);
	elem.addEventListener('drop', handleDrop, false);
	elem.addEventListener('dragend', handleDragEnd, false);
}

function refreshOrderItems(target) {
	const columns = $(target).children();
	let c = 0;
	$.each(columns, function (index, col) {
		$(col).attr("order", c++);
	});
}

var cols = document.querySelectorAll('#columns .column');
[].forEach.call(cols, addDnDHandlers);