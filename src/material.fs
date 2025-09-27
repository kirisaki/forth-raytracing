begin-structure material%
  field: material-type
  ffield: albedo
  ffield: fuzz
end-structure

\ Material types
0 constant lambertian
1 constant metal
2 constant dielectric

\ Initialize material
: material-init! ( addr type -- addr ) ( f-albedo f-fuzz -- )
  >r
  dup material-type r> swap !
  dup albedo f!
  dup fuzz f!
;

\ New material
: material-new ( type -- addr ) ( f-albedo f-fuzz -- )
  >r
  material% allocate throw
  r>
  material-init!
;

\ Allocate a material and initialize it
: material-alloc ( type "name" -- ) ( f-albedo f-fuzz -- )
  create
    material-new ,
  does> ( -- addr ) @
;

\ Free a material
: material-free ( addr -- ) free throw ;

\ Display a material
: .material ( m -- )
  ." Material:" cr
  s" type: " type cr
  dup material-type @ . cr
  s" albedo: " type cr
  dup albedo f@ f. cr
  s" fuzz: " type cr cr
  fuzz f@ f. cr
  cr
;

: test-material ( -- )
  s" ---material test---" type cr
  metal 0.5e 0.3e material-new locals| m |
  m .material
;