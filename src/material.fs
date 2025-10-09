begin-structure material%
  field: m-type
  vec3% +field m-albedo
  ffield: m-fuzz
  ffield: m-ref
end-structure

\ Material types
0 constant lambertian
1 constant metal
2 constant dielectric

\ Initialize material pool
: material-pool-create ( arena -- pool )
  material% 8 pool-init
;

\ New material
: material-new ( type albedo mp -- addr ) ( f-fuzz f-ref -- )
  pool-alloc >r
  r@ m-albedo vec3-move
  r@ m-type !
  r@ m-ref f!
  r@ m-fuzz f!
  r>
;

\ Free a material
: material-free ( addr -- ) free throw ;

\ Display a material
: .material ( m -- )
  ." Material:" cr
  s" type: " type cr
  dup m-type @ . cr
  s" albedo: " type cr
  dup m-albedo .v cr
  s" fuzz: " type cr cr
  m-fuzz f@ f. cr
  s" ref-index: " type cr
  m-ref f@ f. cr
  cr
;

\ Schlieren approximation for reflectance
: schlick ( -- ) ( cos ref-index -- reflectance )
  fdup 1e fswap f- fswap 1e f+ f/ \ cos r0
  fdup f* \ cos r0^2
  fdup 1e fswap f- 1e 3 fpick f- 5e f** f* f+ fnip
;


: scatter ( mat ray rec ray-out att-aout vp rng -- flag rng ) 
  locals| gen vp att-out ray-out rec ray mat |
  mat m-type @
  case
    lambertian of
      vp vec3-zero locals| r |
      gen r vrand-in-unit-sphere to gen
      r rec h-normal v+=
      rec h-point ray-out r-origin vec3-move
      r ray-out r-direction vec3-move
      mat m-albedo att-out vec3-move
      true
      gen
      r vp pool-free
    endof
    metal of
      vp vec3-zero vp vec3-zero vp vec3-zero locals| r reflected rndv |
      ray r-direction r vunit
      r rec h-normal reflected vreflect
      gen rndv vrand-in-unit-sphere to gen
      rndv mat m-fuzz f@ vmul=
      reflected rndv v+=
      rec h-point ray-out r-origin vec3-move
      reflected ray-out r-direction vec3-move
      mat m-albedo att-out vec3-move
      ray-out r-direction rec h-normal vdot f0>
      gen
      r vp pool-free
      reflected vp pool-free
      rndv vp pool-free
    endof
    dielectric of
      rec h-front-face @ if
        1e mat m-ref f@ f/ 
      else
        mat m-ref f@ 
      then
      fdup
      vp vec3-zero vp vec3-zero vp vec3-zero locals| r s ref |
      ray r-direction r vunit -1e r s vmul s rec h-normal vdot 1e fmin \ etai etai cos
      fdup fdup f* 1e fswap f- fabs fsqrt \ etai etai cos sin
      2 fpick f* 1e f> if \ etai etai cos
        \ total internal reflection
        r rec h-normal ref vreflect
        rec h-point ray-out r-origin vec3-move
        r ray-out r-direction vec3-move
        1e 1e 1e att-out v!
        true
        fdrop fdrop fdrop
      else
        fswap schlick gen frand to gen f> if
          \ reflect
          r rec h-normal ref vreflect
          rec h-point ray-out r-origin vec3-move
          ref ray-out r-direction vec3-move
          1e 1e 1e att-out v!
          true
          fdrop 
        else
          \ refraction
          r rec h-normal ref vp vrefract
          rec h-point ray-out r-origin vec3-move
          ref ray-out r-direction vec3-move
          1e 1e 1e att-out v!
          true
        then
      then
      gen
      r vp pool-free
      s vp pool-free
      ref vp pool-free
    endof
  endcase
;

\ Tests
: test-material ( -- )
  s" ---material test" type cr
  1024 arena-create locals| arena |
  arena material-pool-create locals| mp |
  arena vec3-pool-create locals| vp |

  s" Display material:" type cr
  metal 0.5e 0.3e 0.2e vp vec3-new 0.0e 0.0e mp material-new locals| m |
  m .material

  check-stacks
;