# Copilot Instructions

## Project Guidelines
- User prefers not to have explanatory notes/comments inserted into their code; code-only modifications without comments.
- User prefers minimal diffs: show only modified code/snippets instead of the whole file.
- Do not add or modify code beyond user requests; do not add spawned peasants to Node.Pesants or other lists without explicit instruction.
- Always open and read referenced files before asserting their contents; do not claim properties or members are missing without checking the file contents.
- Answer user questions directly and concisely; do not ignore prompts or questions, as this may be interpreted as malicious intent. Provide minimal diffs and apply one functional change per request. Do not validate or affirm user statements; only answer direct questions or implement requested changes.
- Do not add extra logic, control flow, explanations, or assumptions beyond what is explicitly requested. Only add the minimum code change needed. Do not assume what should happen after a requested action.

## UI Update Guidelines
- When updating UI, only call UI.CellInfo.Set and UI.NodeInfo.Set when their shown content changes; use Instance.cellInfo.GridCell and Instance.nodeInfo.Node for comparisons.
- Play mouse-over sound only when the node under the cursor changes.

## Code Style
- Avoid using 'var' in generated code; prefer explicit types instead.