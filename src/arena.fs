\ Default alignment (8 bytes)
8 constant default-align

\ Round up u to the next multiple of align
: align-up ( u align -- u' )
  tuck 1- + swap negate and ;

begin-structure arena%
  field: a-base   \ base address
  field: a-size   \ total size (bytes)
  field: a-off    \ used offset (bytes)
end-structure

\ Display arena info
: .arena ( arena -- )
  cr ." Arena Info:" cr
  ." Base address: " dup a-base @ . cr
  ." Total size: " dup a-size @ . ." bytes" cr
  ." Used offset: " dup a-off @ . ." bytes" cr
  ." Available: " dup a-size @ swap a-off @ - . ." bytes" cr
;

\ Create a new arena with given size
: arena-create ( bytes -- arena )
  locals| bytes |
  arena% allocate throw locals| arena |
  bytes allocate throw arena a-base !
  bytes arena a-size !
  0 arena a-off !
  arena
;

\ Destroy the arena and free all memory
: arena-destroy ( arena -- )
  dup a-base @ free throw
  free throw ;

\ Reset the arena (make all memory available again)
: arena-reset ( arena -- )
  0 swap a-off ! ;

\ Allocate n bytes from the arena with specified alignment
: arena-alloc ( n align arena -- addr | 0 )
  locals| arena align n |
  arena a-off @ align align-up locals| aligned-off |
  aligned-off n + locals| end-off |

  end-off arena a-size @ u> if
    0
  else
    end-off arena a-off !
    arena a-base @ aligned-off +
  then
;

\ Allocate n bytes with default alignment
: arena-alloc-aligned ( n arena -- addr | 0 )
  default-align swap arena-alloc ;

\ Mark the current position of the arena
: arena-mark ( arena -- mark )
  a-off @ ;

\ Rollback the arena to the given mark
: arena-rollback ( mark arena -- )
  a-off ! ;

\ Get available space in arena
: arena-available ( arena -- bytes )
  dup a-size @ swap a-off @ - ;

\ Test arena allocator
: test-arena ( -- )
  cr ." ---test-arena" cr
  
  \ Create 1KB arena
  1024 arena-create locals| arena |
  
  ." Created 1KB arena" cr
  ." Available: " arena arena-available . ." bytes" cr cr
  \ Allocate some memory
  ." Allocating 100 bytes..." cr
  100 arena arena-alloc-aligned dup locals| ptr1 |
  if
    ." Success! Address: " ptr1 . cr
    ." Available: " arena arena-available . ." bytes" cr
  else
    ." Failed!" cr
  then
  cr
  
  \ Mark and allocate more
  ." Marking position..." cr
  arena arena-mark locals| mark |
  
  200 arena arena-alloc-aligned dup locals| ptr2 |
  if
    ." Allocated 200 bytes at: " ptr2 . cr
    ." Available: " arena arena-available . ." bytes" cr
  then
  cr
  
  \ Rollback
  ." Rolling back to mark..." cr
  mark arena arena-rollback
  ." Available: " arena arena-available . ." bytes" cr cr
  
  \ Try to allocate too much
  ." Trying to allocate 2000 bytes (should fail)..." cr
  2000 arena arena-alloc-aligned
  if
    ." Unexpected success!" cr
  else
    ." Failed as expected" cr
  then
  cr
  
  \ Reset arena
  ." Resetting arena..." cr
  arena arena-reset
  ." Available: " arena arena-available . ." bytes" cr cr
  
  \ Cleanup
  arena arena-destroy
  ." Arena destroyed" cr
;