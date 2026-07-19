# Derived TF2 image that bakes our plugin + configs in a staging dir OUTSIDE the
# game volume, then copies them into place at boot AFTER the base entry.sh has
# installed MetaMod/SourceMod. This avoids the bind-mount ownership trap that
# skips the SM install (base entry.sh only installs SM if addons/sourcemod is
# absent, and a bind mount creates that dir root-owned before install runs).
FROM cm2network/tf2:sourcemod

# Staged content (baked into the image, never shadowed by the volume mount).
COPY --chown=steam:steam cfg/ /ailobby/cfg/
COPY --chown=steam:steam addons-overlay/sourcemod/plugins/ailobby.smx /ailobby/plugins/ailobby.smx

# Inject a copy step immediately before srcds launches. `i\` inserts a literal
# line; the ${...} vars are expanded by bash at runtime, not by sed.
USER root
RUN sed -i '\#bash "${STEAMAPPDIR}/srcds_run"#i cp -f /ailobby/plugins/*.smx "${STEAMAPPDIR}/${STEAMAPP}/addons/sourcemod/plugins/"; cp -rf /ailobby/cfg/. "${STEAMAPPDIR}/${STEAMAPP}/cfg/"; echo "[ailobby] staged plugin + cfgs into game dir";' /home/steam/entry.sh \
    && grep -q "\[ailobby\] staged plugin" /home/steam/entry.sh
USER steam
