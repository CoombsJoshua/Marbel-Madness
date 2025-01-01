extends Panel

var time: float = 0.0
var minutes: int = 0
var seconds: int = 0
var msec: int = 0

func _process(delta):
	time += delta
	msec = int(fmod(time, 1) * 100)  # Correct milliseconds calculation
	seconds = int(fmod(time, 60))  # Get seconds
	minutes = int(fmod(time, 3600) / 60)  # Get minutes
	$Minutes.text = "%02d:" % minutes
	$Seconds.text = "%02d:" % seconds
	$Msec.text = "%02d" % msec  # For milliseconds, keep it 3 digits
	

func stop():
	set_process(false)

func get_time_formatted() -> String:
	return "%02d:%02d.%03d" % [minutes, seconds, msec]
