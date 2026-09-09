execution-verb-name = Execute
execution-verb-message = Use your weapon to execute someone.

# All the below localisation strings have access to the following variables
# attacker (the person committing the execution)
# victim (the person being executed)
# tool (the tool used for the execution)

execution-before-popup-ballistic = {CAPITALIZE($attacker)} presses the barrel of {THE($tool)} against the side of {$victim}'s head.
execution-before-popup-ballistic-self = {CAPITALIZE($attacker)} presses the barrel of {THE($tool)} against the side of {POSS-ADJ($victim)} head.
execution-after-popup-ballistic = {CAPITALIZE($attacker)} shoots {$victim} in the head!
execution-after-popup-ballistic-self = {CAPITALIZE($attacker)} shoots {REFLEXIVE($victim)} in the head!