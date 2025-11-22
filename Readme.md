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
Zarovna 1 superproject

Zarovna viacero superprojektov

Conflict

Branch not existing locally

Submodule nema dev

Superprojekt nema dev

Histories diverged on the remote.
Should fail with exception