begin-structure node%
  field: next-node
  field: val
end-structure


\ Construct a node with value x
: <node>  ( x -- node )
  node% allocate throw >r
  0 r@ next-node !
  r@ val !
  r>
 ;

\ Push node to front
: push-front  ( x head -- head' )
  swap <node> dup >r
  next-node !
  r>
;

\ Pop node from front
: pop-front  ( head -- head' x | 0 0 )
  dup 0= if 0 0 exit then
  dup >r
  r@ next-node @
  r@ val @
  r@ free throw
  r> drop
;

\ For each node in the list
: foreach  ( head xt -- )
  >r
  begin
    dup 0<>
  while
    dup val @ r@ execute
    next-node @
  repeat
  drop r> drop
;

: .val  ( x -- )  . space ;


: test-list ( -- )
  cr ." ---test-list" cr

  0 locals| head |

  ." push-front: 10 20 30" cr
  10 head push-front to head
  20 head push-front to head
  30 head push-front to head

  ." list (foreach): "  head ['] .val foreach  cr

  head pop-front   ( h' x | 0 0 )
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then

  head pop-front
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then

  head pop-front
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then

  head pop-front
  2dup 0= swap 0= and if 2drop ." pop-front (empty): EMPTY" cr
                   else swap to head ." pop-front (error): " . cr then


  ." done." cr
;