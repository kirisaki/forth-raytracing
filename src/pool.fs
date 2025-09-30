\ Pool structure
begin-structure pool%
  field: p-arena   \ backing arena
  field: p-elem    \ element size (bytes)
  field: p-align   \ alignment
  field: p-free    \ free-list head (addr|0)
end-structure

\ Create a new pool backed by an arena
\ Note: elem-size must be >= 1 cells for free-list links
: pool-init ( arena elem-size align -- pool )
  locals| align elem arena |
  elem 1 cells < if
    drop
    ." Error: element size must be >= " 1 cells . ." bytes" cr
    0
  else
    pool% allocate throw locals| pool |
    arena pool p-arena !
    elem pool p-elem !
    align pool p-align !
    0 pool p-free !
    pool
  then
;

\ Destroy the pool structure (does not destroy backing arena)
: pool-destroy ( pool -- )
  free throw ;

\ Reset the pool (clears free list)
\ Note: This does not free the memory in the arena
: pool-reset ( pool -- )
  0 swap p-free ! ;

\ Allocate one element from the pool
: pool-alloc ( pool -- addr | 0 )
  dup p-free @ ?dup if
    \ Free-list is not empty - pop from free list
    ( pool free-node )
    dup @                      ( pool free-node next )
    rot p-free !               ( free-node )
  else
    \ Free-list is empty - allocate from arena
    dup p-elem @               ( pool elem-size )
    over p-align @             ( pool elem-size align )
    rot p-arena @              ( elem-size align arena )
    arena-alloc                ( addr | 0 )
  then ;

\ Return an element to the pool's free list
: pool-free ( addr pool -- )
  dup p-free @                 ( addr pool old-head )
  rot                          ( pool old-head addr )
  tuck !                       ( pool addr )
  swap p-free ! ;

\ Get statistics about pool usage
: pool-stats ( pool -- )
  cr ." Pool Statistics:" cr
  ." Element size: " dup p-elem @ . ." bytes" cr
  ." Alignment: " dup p-align @ . ." bytes" cr
  ." Free list: "
  p-free @ ?dup if
    0 swap                     ( count node )
    begin
      ?dup
    while
      swap 1+ swap             ( count+1 node )
      @                        ( count node-next )
    repeat
    . ." elements" cr
  else
    ." empty" cr
  then
;

\ Test pool allocator
: test-pool ( -- )
  cr ." ---test-pool" cr
  
  \ Create backing arena (1KB)
  1024 arena-create locals| arena |
  ." Created 1KB backing arena" cr cr
  
  \ Create pool for 16-byte elements
  arena 16 8 pool-init locals| pool |
  ." Created pool (16 bytes/element, 8-byte aligned)" cr
  pool pool-stats cr
  
  \ Allocate some elements
  ." Allocating 3 elements..." cr
  pool pool-alloc locals| p1 |
  pool pool-alloc locals| p2 |
  pool pool-alloc locals| p3 |
  p1 . p2 . p3 . cr
  pool pool-stats cr
  
  \ Free one element
  ." Freeing middle element..." cr
  p2 pool pool-free
  pool pool-stats cr
  
  \ Allocate again (should reuse freed element)
  ." Allocating again (should reuse)..." cr
  pool pool-alloc locals| p4 |
  ." Got: " p4 . 
  p4 p2 = if ." (reused p2!)" else ." (new allocation)" then cr
  pool pool-stats cr
  
  \ Free all
  ." Freeing all elements..." cr
  p1 pool pool-free
  p3 pool pool-free
  p4 pool pool-free
  pool pool-stats cr
  
  \ Reset pool
  ." Resetting pool..." cr
  pool pool-reset
  pool pool-stats cr
  
  \ Test error case - element too small
  ." Testing element size < cell size..." cr
  arena 2 4 pool-init ?dup if
    ." Unexpected success!" cr
    pool-destroy
  else
    ." Failed as expected" cr
  then
  cr
  
  \ Cleanup
  pool pool-destroy
  arena arena-destroy
  ." Cleanup complete" cr
;

\ Example: Pool for vec3% structures
\ : vec3-pool-create ( arena -- pool )
\   vec3% 8 pool-init ;
\
\ Usage:
\   1024 arena-create constant my-arena
\   my-arena vec3-pool-create constant vec-pool
\   vec-pool pool-alloc ( -- vec-addr | 0 )
\   vec-addr vec-pool pool-free