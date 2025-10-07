begin-structure node%
  field: n-next
  field: n-val
end-structure


\ Construct a node with value x
: <node>  ( x pool -- node )
  locals| p x |
  p pool-alloc locals| r |
  0 r n-next !
  x r n-val !
  r
 ;

\ Push node to front
: push-front  ( x head pool -- head' )
  locals| p h x |
  x p <node> >r
  h r@ n-next !
  r>
;

\ Pop node from front
: pop-front  ( head pool -- head' x | 0 0 )
  locals| p h |
  h 0= if 0 0 exit then
  h n-next @ locals| h' |
  h n-val @
  h p pool-free
  h' swap
;

\ Iterate over list, calling xt for each value
: foreach  ( head xt -- )
  locals| xt |
  begin
    dup 0<>
  while
    dup >r n-val @ xt execute r> n-next @
  repeat
  drop 
;

\ Free all nodes in the list
: free-list  ( head pool -- )
  locals| p |
  begin
    dup 0<>
  while
    >r
    n-next @
    r@ p pool-free
    r> drop
  repeat
  drop
;

: test-list ( -- )
  cr ." ---test-list" cr

  0 locals| head |
  1024 arena-create locals| arena |
  arena node% 8 pool-init locals| pool |

  ." push-front: 10 20 30" cr
  10 head pool push-front to head 
  20 head pool push-front to head 
  30 head pool push-front to head 

  ." list (foreach): "  head [: . space ;] foreach  cr
  ." list (foreach/env3): "  
  1 2 3  head  [: >r dup 2over rot r> + + + . ;] foreach 3 0 do drop loop cr
  head pool  pop-front   ( h' x | 0 0 )
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then
  head pool pop-front
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then

  head pool pop-front
  2dup 0= swap 0= and if 2drop ." pop-front: EMPTY" cr  else swap to head ." pop-front: " . cr then

  head pool pop-front
  2dup 0= swap 0= and if 2drop ." pop-front (empty): EMPTY" cr
                   else swap to head ." pop-front (error): " . cr then

  head pool free-list
  arena arena-destroy

  check-stacks
;