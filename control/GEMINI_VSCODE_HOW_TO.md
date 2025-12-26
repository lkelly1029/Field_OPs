# GEMINI_VSCODE_HOW_TO.md

## Goal
Use ONE file as the single instruction entrypoint for Gemini in VS Code.

## Files
- GEMINI_AGENT_BRIEF.md  (the entrypoint; Gemini reads this first)
- STATUS.md              (Gemini updates this)

## Steps (Windows / VS Code)
1) Copy these files into your repo root:
   C:\Users\trulu\Field_OPs
   - GEMINI_AGENT_BRIEF.md
   - STATUS.md

2) In VS Code, open `GEMINI_AGENT_BRIEF.md`.

3) In Gemini chat, send ONE message:
   "Follow the instructions in this file exactly. Read all Sources of Truth, scan the repo, then update STATUS.md."

4) Gemini should respond with:
   - Findings
   - Next steps
   - STATUS.md update (applied)

## If Gemini starts inventing or editing other files
Reply:
"Stop. Only update STATUS.md. Do not modify any other files. Re-run using the Operating Rules in GEMINI_AGENT_BRIEF.md."