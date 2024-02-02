var instance = null;
var callback = null;
var xDown = null;
var yDown = null;
function addSwipeListener(instance, callback) {
    this.instance = instance;
    this.callback = callback;
}


document.addEventListener('touchstart', handleTouchStart, false);
document.addEventListener('touchmove', handleTouchMove, false);

function handleTouchStart(evt) {
    const firstTouch = evt.touches[0];
    xDown = firstTouch.clientX;
    yDown = firstTouch.clientY;
}

function handleTouchMove(evt) {

    if ( ! xDown || ! yDown ) return;
    
    var xDiff = xDown - evt.touches[0].clientX;
    var yDiff = yDown - evt.touches[0].clientY;

    if ( Math.abs( xDiff ) > Math.abs( yDiff ) ) {
        if ( xDiff > 0 ) {
            instance.invokeMethodAsync(callback, "right");
        } else {
            instance.invokeMethodAsync(callback, "left");
        }
    }

    xDown = null;
    yDown = null;
}
