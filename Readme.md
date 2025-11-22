## Functions

### Kontrola zarovnania submodulov(Idempotentná)
Kontrola či submoduly ukazujú správne. 

Správna ak 
Superprojekt DEV Reference commit = Subomdule DEV HEAD

V uvahu sa beru iba remote branches.

### Zarovnanie submodulu napriec superprojektami(Idempotentná?)

Príklad použitia:
Pushol som commit do biosignCore. V rámci storky som forwardol Biosign.
Táto funkcionalita ho forwardne aj v DES.

Algoritmus

1. chcem byt na dev v superprojekte
2. checkout dev v submodule
3. Fetch and merge
4. Forward commit v superprojekte

Chcem vzdy remote branches?
Branch.TIP
- Remote ma co chcem
- Local.TIP = Remote.TIP?

Chcem remote branchces
Ak lokalne neexistuje, nemam sa kam pozriet.

Fast forward strategy is used. Histories diverged. It needs to handled mannualy

## Testing cases

### Aligning superproject
Umoznuje zarovnat 1 superprojekt alebo vsetky superprojekty.

Conflict
- exception je vyhodne

Superprojekt nema branchu na ktorej je treba zarovnat submodule.
- git switch ju vytvori

Submodul nema branchu na ktorej head sa treba referencovat
- git switch ju vytvory

Histories diverged on the remote.
NOT YET TESTED

Pozor: Ak ma aplikacia zakazane pushovat a lokalne branchce smeruju spravne, user dostane exception.
Pretoze applikacia kontroluje sa pozera na remote branchce.
Dovod: Origin stav nesedi. Ale taktiez nema co comitnut pretoze na lokal to ukazuje dobre.