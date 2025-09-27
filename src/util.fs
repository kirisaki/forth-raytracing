\ Clamp a floating point number to a maximum value
: fclamp-max ( -- ) ( n max -- n' ) 
  fdup 2 fpick f< if
    fnip
  else
    fdrop
  then
;

\ Clamp a floating point number to a minimum value
: fclamp-min ( -- ) ( n min -- n' )
  fdup 2 fpick f> if
    fnip
  else
    fdrop
  then
;

\ Clamp a floating point number to a range
: fclamp ( -- ) ( n min max -- n' )
  2 fpick 2 fpick fclamp-min
  fover fclamp-max
  fnip fnip fnip
;

: test-clamp
  ." ---test-clamp" cr
  ." 0.5 clamped to 1.0: "
  0.5e 1.0e fclamp-max f. cr
  ." 1.5 clamped to 1.0: "
  1.5e 1.0e fclamp-max f. cr
  ." -0.5 clamped to 0.0: "
  -0.5e 0.0e fclamp-min f. cr
  ." 0.5 clamped to 0.0: "
  0.5e 0.0e fclamp-min f. cr
  cr
  ." 0.5 clamped to [0.0, 1.0]: "
  0.5e 0.0e 1.0e fclamp f. cr
  ." -0.5 clamped to [0.0, 1.0]: "
  -0.5e 0.0e 1.0e fclamp f. cr
  ." 1.5 clamped to [0.0, 1.0]: "
  1.5e 0.0e 1.0e fclamp f. cr
;

\ Bye when the length of the stack greater than n
: bye< ( n -- )
  1+ depth < if
    cr
    .s cr
    bye
  then
;

\ Log with the stack
: log ( c-addr u -- )
  type ." :" .s cr
;

\ Log when the length of the stack greater than n
: log< ( c-addr u n -- )
  2 + depth < if
    log
  else
    2drop
  then
;