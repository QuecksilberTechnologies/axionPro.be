import fs from "node:fs/promises";
import path from "node:path";

const workspace = process.cwd();
const frontendRoot = "C:/latestAxionProUI/axionpro-app/src";
const backendRoot = path.join(workspace, "axionpro.api", "Controllers");
const reportPath = path.join(workspace, "API-Documentation-Sync-Report.md");
const applyDocs = process.argv.includes("--apply");
const commentUnused = process.argv.includes("--comment-unused");
const dryRun = process.argv.includes("--dry-run");
const applyChanges = (applyDocs || commentUnused) && !dryRun;

async function walk(directory, predicate) {
  const output = [];
  for (const entry of await fs.readdir(directory, { withFileTypes: true })) {
    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) output.push(...await walk(fullPath, predicate));
    else if (predicate(fullPath)) output.push(fullPath);
  }
  return output;
}

function stripCommentsKeepLength(source) {
  let result = "";
  let state = "code";
  for (let index = 0; index < source.length; index++) {
    const character = source[index];
    const next = source[index + 1];
    if (state === "code") {
      if (character === "/" && next === "/") { result += "  "; index++; state = "line"; }
      else if (character === "/" && next === "*") { result += "  "; index++; state = "block"; }
      else if (character === "'") { result += character; state = "single"; }
      else if (character === '"') { result += character; state = "double"; }
      else if (character === "`") { result += character; state = "template"; }
      else result += character;
    } else if (state === "line") {
      result += character === "\n" || character === "\r" ? character : " ";
      if (character === "\n") state = "code";
    } else if (state === "block") {
      if (character === "*" && next === "/") { result += "  "; index++; state = "code"; }
      else result += character === "\n" || character === "\r" ? character : " ";
    } else {
      result += character;
      if (character === "\\" && index + 1 < source.length) { result += source[++index]; continue; }
      if ((state === "single" && character === "'") || (state === "double" && character === '"') || (state === "template" && character === "`")) state = "code";
    }
  }
  return result;
}

function relativeTo(root, file) {
  return path.relative(root, file).replaceAll("\\", "/");
}

function lineAt(source, index) {
  return source.slice(0, index).split(/\r?\n/).length;
}

function normalizeRoute(value) {
  if (!value) return "";
  let route = String(value).trim().replaceAll("\\", "/");
  route = route.replace(/[?#].*$/, "");
  route = route.replace(/^https?:\/\/[^/]+/i, "");
  route = route.replace(/^.*?\/api\//i, "");
  route = route.replace(/^api\//i, "");
  route = route.replace(/\{[^}]+\}/g, "{}");
  return route.replace(/^\/+|\/+$/g, "").replace(/\/+/g, "/").toLowerCase();
}

function routeKey(verb, route) {
  return `${verb.toUpperCase()}|${normalizeRoute(route)}`;
}

function escapeRegex(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function xmlEscape(value) {
  return String(value).replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");
}

function markdownEscape(value) {
  return String(value).replaceAll("|", "\\|").replaceAll("\r", " ").replaceAll("\n", "<br>");
}

function unique(values, maximum = 8) {
  return [...new Set(values.filter(Boolean))].slice(0, maximum);
}

function words(name) {
  return String(name)
    .replace(/([a-z0-9])([A-Z])/g, "$1 $2")
    .replace(/[_#-]+/g, " ")
    .replace(/\s+/g, " ")
    .trim();
}

function inferPurpose(methodName) {
  const label = words(methodName).toLowerCase();
  if (/^(get|list|load|fetch|find|search|view|detail|option|ddl)/.test(methodName.toLowerCase())) return `retrieves ${label.replace(/^(get|list|load|fetch|find|search|view|detail|option|ddl)\s*/, "") || "data"}`;
  if (/^(create|add|register|open)/.test(methodName.toLowerCase())) return `creates ${label.replace(/^(create|add|register|open)\s*/, "") || "data"}`;
  if (/^(update|edit|change|set|save)/.test(methodName.toLowerCase())) return `updates ${label.replace(/^(update|edit|change|set|save)\s*/, "") || "data"}`;
  if (/^(delete|remove)/.test(methodName.toLowerCase())) return `deletes ${label.replace(/^(delete|remove)\s*/, "") || "data"}`;
  if (/^(assign|map|link)/.test(methodName.toLowerCase())) return `assigns or maps ${label.replace(/^(assign|map|link)\s*/, "") || "data"}`;
  if (/^(login|sign in|authenticate)/.test(label)) return "authenticates the user";
  if (/^(validate|verify)/.test(methodName.toLowerCase())) return `validates ${label.replace(/^(validate|verify)\s*/, "") || "data"}`;
  return `performs the Angular function ${label}`;
}

function extractFirstArgument(source, startIndex) {
  let quote = null;
  let nesting = 0;
  for (let index = startIndex; index < source.length; index++) {
    const character = source[index];
    if (quote) {
      if (character === "\\") { index++; continue; }
      if (character === quote) quote = null;
      continue;
    }
    if (character === "'" || character === '"' || character === "`") { quote = character; continue; }
    if (character === "(" || character === "[" || character === "{") nesting++;
    if (character === ")" || character === "]" || character === "}") {
      if (nesting === 0 && character === ")") return source.slice(startIndex, index);
      nesting--;
    }
    if (character === "," && nesting === 0) return source.slice(startIndex, index);
  }
  return source.slice(startIndex);
}

function buildDefinitions(source) {
  const definitions = new Map();
  const definitionPattern = /(?:const|let|var|readonly|private\s+readonly|protected\s+readonly)\s+(?<name>#?[A-Za-z_$][\w$]*)\s*=\s*(?<value>[^;\r\n]+(?:`[\s\S]*?`)?[^;\r\n]*);/g;
  for (const match of source.matchAll(definitionPattern)) {
    const name = match.groups.name;
    definitions.set(name, match.groups.value.trim());
    definitions.set(name.replace(/^#/, ""), match.groups.value.trim());
  }
  return definitions;
}

function resolveUrlExpression(expression, definitions, seen = new Set()) {
  let value = expression.trim();
  const resolveDefinition = (name) => {
    const canonical = definitions.has(name) ? name : name.replace(/^#/, "");
    const definition = definitions.get(canonical);
    if (!definition || seen.has(canonical)) return `{${name}}`;
    return resolveUrlExpression(definition, definitions, new Set([...seen, canonical]));
  };
  value = value.replace(/\$\{\s*(?:this\.)?(?<name>#?[A-Za-z_$][\w$]*)\s*\}/g, (_match, _name, _offset, _source, groups) => resolveDefinition(groups.name));
  value = value.replace(/this\.(?<name>#?[A-Za-z_$][\w$]*)/g, (_match, _name, _offset, _source, groups) => `(${resolveDefinition(groups.name)})`);
  if (/^#?[A-Za-z_$][\w$]*$/.test(value)) value = resolveDefinition(value);
  value = value.replace(/\$\{\s*environment\.[^}]+\s*\}/g, "");
  value = value.replace(/environment\.[A-Za-z_$][\w$]*/g, "");
  value = value.replace(/\$\{[^}]+\}/g, "{}");
  value = value.replace(/["'`\s()+]/g, "");
  return value;
}

function findContainingFunction(source, index) {
  const prefix = source.slice(Math.max(0, index - 8000), index);
  const functionPattern = /(?:^|[;\r\n]\s*)(?:public\s+|private\s+|protected\s+|readonly\s+)?(?<name>[A-Za-z_$][\w$]*)\s*(?:<[^{};()]*>)?\s*\([^{};]*\)\s*(?::\s*[^={;]+)?\s*\{/gm;
  let result = null;
  for (const match of prefix.matchAll(functionPattern)) result = match;
  return result?.groups?.name ?? "anonymousHttpCall";
}

function parseImports(source) {
  const imported = [];
  for (const match of source.matchAll(/import\s+{(?<names>[^}]+)}/g)) {
    for (const name of match.groups.names.split(",")) imported.push(name.trim().split(/\s+as\s+/i)[0].trim());
  }
  return imported.filter(Boolean);
}

async function readAngularFiles() {
  const files = await walk(frontendRoot, (file) => file.endsWith(".ts") && !file.endsWith(".spec.ts"));
  const records = [];
  for (const file of files) {
    const raw = await fs.readFile(file, "utf8");
    const active = stripCommentsKeepLength(raw);
    const className = /export\s+(?:default\s+)?(?:abstract\s+)?class\s+(?<name>[A-Za-z_$][\w$]*)/.exec(active)?.groups?.name
      ?? /export\s+const\s+(?<name>[A-Za-z_$][\w$]*)/.exec(active)?.groups?.name
      ?? null;
    records.push({ file, relative: relativeTo(frontendRoot, file), raw, active, className, imports: parseImports(active) });
  }
  return records;
}

function extractAngularCalls(record) {
  const definitions = buildDefinitions(record.active);
  const calls = [];
  const httpCall = /\.\s*(?<verb>get|post|put|delete|patch)\s*(?:<[^>]*>)?\s*\(/gi;
  for (const match of record.active.matchAll(httpCall)) {
    const openIndex = match.index + match[0].lastIndexOf("(") + 1;
    const expression = extractFirstArgument(record.active, openIndex);
    const resolvedExpression = resolveUrlExpression(expression, definitions);
    const route = normalizeRoute(resolvedExpression);
    if (!route) continue;
    calls.push({
      verb: match.groups.verb.toUpperCase(), route, file: record.file, relative: record.relative,
      line: lineAt(record.raw, match.index), className: record.className ?? "AngularModule",
      methodName: findContainingFunction(record.active, match.index), purpose: inferPurpose(findContainingFunction(record.active, match.index)),
    });
  }
  return calls;
}

function combineControllerRoute(controllerRoute, actionRoute, controllerName) {
  const base = (controllerRoute || "api/[controller]").replace(/\[controller\]/ig, controllerName);
  if (!actionRoute) return normalizeRoute(base);
  if (/^(?:~\/|\/)/.test(actionRoute)) return normalizeRoute(actionRoute.replace(/^~\//, ""));
  return normalizeRoute(`${base}/${actionRoute}`);
}

async function readBackendEndpoints() {
  const files = await walk(backendRoot, (file) => file.endsWith("Controller.cs"));
  const endpoints = [];
  for (const file of files) {
    const raw = await fs.readFile(file, "utf8");
    const active = stripCommentsKeepLength(raw);
    const className = /public\s+(?:sealed\s+)?class\s+(?<name>[A-Za-z_$][\w$]*Controller)\b/.exec(active)?.groups?.name;
    if (!className) continue;
    const controllerName = className.replace(/Controller$/, "");
    const controllerRoute = /\[\s*Route\s*\(\s*"(?<route>[^"]*)"/.exec(active)?.groups?.route ?? "api/[controller]";
    const httpAttribute = /\[\s*Http(?<verb>Get|Post|Put|Delete|Patch|Head|Options)\s*(?:\(\s*"(?<route>[^"]*)")?[^\]]*\]/g;
    for (const match of active.matchAll(httpAttribute)) {
      const after = active.slice(match.index + match[0].length, match.index + match[0].length + 5000);
      const actionMatch = /public\s+(?:async\s+)?[A-Za-z_$][\w$<>,?.\[\]\s]*?\s+(?<name>[A-Za-z_$][\w$]*)\s*\(/.exec(after);
      if (!actionMatch) continue;
      const actionIndex = match.index + match[0].length + actionMatch.index;
      const actionBodyStart = active.indexOf("{", actionIndex);
      const actionEnd = actionBodyStart >= 0 ? findClosingBrace(active, actionBodyStart) + 1 : -1;
      if (actionEnd <= actionBodyStart) continue;
      endpoints.push({
        file, relative: relativeTo(workspace, file), raw, className, action: actionMatch.groups.name,
        verb: match.groups.verb.toUpperCase(), route: combineControllerRoute(controllerRoute, match.groups.route ?? "", controllerName),
        attributeIndex: match.index, actionIndex, actionEnd,
      });
    }
  }
  return endpoints.sort((left, right) => routeKey(left.verb, left.route).localeCompare(routeKey(right.verb, right.route)));
}

function resolveModuleFile(fromFile, moduleSpecifier) {
  if (!moduleSpecifier.startsWith(".")) return null;
  return path.resolve(path.dirname(fromFile), `${moduleSpecifier}.ts`);
}

function joinRoute(prefix, segment) {
  return `/${[prefix, segment].flatMap((part) => String(part).split("/")).filter(Boolean).join("/")}`;
}

function findClosingBrace(source, openIndex) {
  let depth = 0;
  let quote = null;
  for (let index = openIndex; index < source.length; index++) {
    const character = source[index];
    if (quote) {
      if (character === "\\") { index++; continue; }
      if (character === quote) quote = null;
      continue;
    }
    if (character === "'" || character === '"' || character === "`") { quote = character; continue; }
    if (character === "{") depth++;
    else if (character === "}" && --depth === 0) return index;
  }
  return -1;
}

function routeObjectBodies(source) {
  const routes = [];
  for (let index = 0; index < source.length; index++) {
    if (source[index] !== "{") continue;
    const close = findClosingBrace(source, index);
    if (close < 0) continue;
    const body = source.slice(index + 1, close);
    const routePath = /^\s*path\s*:\s*["'](?<path>[^"']*)["']/.exec(body);
    if (routePath) routes.push({ path: routePath.groups.path, body });
  }
  return routes;
}

async function buildComponentRoutes() {
  const componentRoutes = new Map();
  const visited = new Set();
  async function visit(routeFile, prefix) {
    if (visited.has(`${routeFile}|${prefix}`)) return;
    visited.add(`${routeFile}|${prefix}`);
    let source;
    try { source = stripCommentsKeepLength(await fs.readFile(routeFile, "utf8")); } catch { return; }
    for (const route of routeObjectBodies(source)) {
      const lazyComponent = /loadComponent\s*:\s*\(\s*\)\s*=>\s*import\s*\(\s*["'](?<module>[^"']+)["']\s*\)(?:\s*\.then\s*\(\s*\(?\s*\w+\s*\)?\s*=>\s*\w+\.(?<component>[A-Za-z_$][\w$]*)\s*\))?/.exec(route.body);
      let component = lazyComponent?.groups?.component;
      if (lazyComponent && !component) {
        const componentFile = resolveModuleFile(routeFile, lazyComponent.groups.module);
        try { component = /export\s+(?:default\s+)?(?:abstract\s+)?class\s+(?<name>[A-Za-z_$][\w$]*)/.exec(await fs.readFile(componentFile, "utf8"))?.groups?.name; } catch { component = null; }
      }
      component ??= /\bcomponent\s*:\s*(?<component>[A-Za-z_$][\w$]*)/.exec(route.body)?.groups?.component;
      if (component) {
        if (!componentRoutes.has(component)) componentRoutes.set(component, new Set());
        componentRoutes.get(component).add(joinRoute(prefix, route.path));
      }
      const childSpecifier = /loadChildren\s*:\s*\(\s*\)\s*=>\s*import\s*\(\s*["'](?<module>[^"']+)["']\s*\)/.exec(route.body)?.groups?.module;
      const child = childSpecifier && resolveModuleFile(routeFile, childSpecifier);
      if (child) await visit(child, joinRoute(prefix, route.path));
    }
  }
  await visit(path.join(frontendRoot, "app", "app.routes.ts"), "");
  return componentRoutes;
}

function buildUsageResolver(angularRecords, componentRoutes) {
  const reverseImports = new Map();
  for (const record of angularRecords) {
    if (!record.className) continue;
    for (const imported of record.imports) {
      if (!reverseImports.has(imported)) reverseImports.set(imported, new Set());
      reverseImports.get(imported).add(record.className);
    }
  }
  const byClass = new Map(angularRecords.filter((record) => record.className).map((record) => [record.className, record]));
  function consumersFor(context) {
    const callPattern = new RegExp(`\\.\\s*${escapeRegex(context.methodName)}\\s*\\(`);
    const direct = angularRecords.filter((record) => record.file !== context.file && record.className && record.imports.includes(context.className) && callPattern.test(record.active));
    const discovered = new Set(direct.map((record) => record.className));
    const queue = [...discovered];
    while (queue.length > 0 && discovered.size < 80) {
      const current = queue.shift();
      for (const importer of reverseImports.get(current) ?? []) {
        if (!discovered.has(importer)) { discovered.add(importer); queue.push(importer); }
      }
    }
    return [...discovered].map((className) => byClass.get(className)).filter(Boolean);
  }
  function pagesFor(context) {
    return unique(consumersFor(context).flatMap((consumer) => [...(componentRoutes.get(consumer.className) ?? [])]));
  }
  return { consumersFor, pagesFor };
}

function findAttributeBlockStart(source, httpAttributeIndex) {
  let start = source.lastIndexOf("\n", httpAttributeIndex - 1) + 1;
  while (start > 0) {
    const previousStart = source.lastIndexOf("\n", start - 2) + 1;
    const previousLine = source.slice(previousStart, start).replace(/[\r\n]/g, "").trim();
    if (!previousLine || previousLine.startsWith("[") || previousLine.endsWith("]") || previousLine.endsWith(",")) { start = previousStart; continue; }
    break;
  }
  return start;
}

function findXmlDocBlock(source, attributeBlockStart) {
  const preceding = source.slice(0, attributeBlockStart);
  const match = /(?<doc>(?:^[ \t]*\/\/\/[^\r\n]*(?:\r?\n|$))+(?:^[ \t]*\r?\n)*)(?![\s\S])/m.exec(preceding);
  return match ? { start: match.index, end: attributeBlockStart } : null;
}

function attributeIndent(endpoint) {
  const lineStart = endpoint.raw.lastIndexOf("\n", endpoint.attributeIndex - 1) + 1;
  return endpoint.raw.slice(lineStart, endpoint.attributeIndex);
}

function lineStart(source, index) {
  return source.lastIndexOf("\n", index - 1) + 1;
}

function lineEnd(source, index) {
  const newline = source.indexOf("\n", index);
  return newline < 0 ? source.length : newline + 1;
}

function commentOutAction(endpoint) {
  const attributeBlockStart = findAttributeBlockStart(endpoint.raw, endpoint.attributeIndex);
  const existingDoc = findXmlDocBlock(endpoint.raw, attributeBlockStart);
  const start = existingDoc?.start ?? attributeBlockStart;
  const block = endpoint.raw.slice(start, endpoint.actionEnd);
  const eol = endpoint.raw.includes("\r\n") ? "\r\n" : "\n";
  const indent = /^[ \t]*/.exec(block)?.[0] ?? "";
  const commented = block.split(/\r?\n/).map((line) => `${indent}// ${line}`).join(eol);
  return { start, end: endpoint.actionEnd, replacement: `${indent}#region Unused${eol}${commented}${eol}${indent}#endregion` };
}

function commentOutHttpAttribute(endpoint) {
  const start = lineStart(endpoint.raw, endpoint.attributeIndex);
  const end = lineEnd(endpoint.raw, endpoint.attributeIndex);
  const line = endpoint.raw.slice(start, end).replace(/\r?\n$/, "");
  const indent = /^[ \t]*/.exec(line)?.[0] ?? "";
  const eol = endpoint.raw.includes("\r\n") ? "\r\n" : "\n";
  return {
    start, end,
    replacement: `${indent}#region Unused${eol}${indent}// ${line.slice(indent.length)}${eol}${indent}#endregion${eol}`,
  };
}

function buildMatchedDoc(endpoint, contexts, usageResolver) {
  const functions = unique(contexts.map((context) => `${context.className}.${context.methodName} (${context.relative}:${context.line})`));
  const purposes = unique(contexts.map((context) => context.purpose));
  const components = unique(contexts.flatMap((context) => usageResolver.consumersFor(context).map((consumer) => `${consumer.className} (${consumer.relative})`)));
  const pages = unique(contexts.flatMap((context) => usageResolver.pagesFor(context)));
  const pageText = pages.length ? pages.join("; ") : "No static Angular route was resolved; see Angular UI component(s).";
  const componentText = components.length ? components.join("; ") : "No consuming Angular component was statically resolved.";
  const indent = attributeIndent(endpoint);
  const eol = endpoint.raw.includes("\r\n") ? "\r\n" : "\n";
  return [
    `${indent}/// <summary>`,
    `${indent}/// Used-In-Angular: ${xmlEscape(purposes.join("; "))}.`,
    `${indent}/// </summary>`,
    `${indent}/// <remarks>`,
    `${indent}/// <para>Angular usage status: Used-In-Angular.</para>`,
    `${indent}/// <para>Angular function(s): ${xmlEscape(functions.join("; "))}.</para>`,
    `${indent}/// <para>Angular purpose: ${xmlEscape(purposes.join("; "))}.</para>`,
    `${indent}/// <para>Integrated UI page(s): ${xmlEscape(pageText)}</para>`,
    `${indent}/// <para>Angular UI component(s): ${xmlEscape(componentText)}</para>`,
    `${indent}/// </remarks>`,
  ].join(eol);
}

function buildNotUsedDoc(endpoint) {
  const indent = attributeIndent(endpoint);
  const eol = endpoint.raw.includes("\r\n") ? "\r\n" : "\n";
  return [
    `${indent}/// <summary>`,
    `${indent}/// Not-Used-In-Angular.`,
    `${indent}/// </summary>`,
    `${indent}/// <remarks>`,
    `${indent}/// <para>Angular usage status: Not-Used-In-Angular.</para>`,
    `${indent}/// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>`,
    `${indent}/// <para>Backend endpoint: ${xmlEscape(`${endpoint.verb} /api/${endpoint.route}`)}.</para>`,
    `${indent}/// </remarks>`,
  ].join(eol);
}

function buildReport(endpoints, angularByKey, usageResolver, componentRoutes, angularCalls) {
  const matched = endpoints.filter((endpoint) => angularByKey.has(routeKey(endpoint.verb, endpoint.route)));
  const unmatched = endpoints.length - matched.length;
  const rows = endpoints.map((endpoint) => {
    const contexts = angularByKey.get(routeKey(endpoint.verb, endpoint.route)) ?? [];
    const used = contexts.length > 0;
    const functions = used ? unique(contexts.map((context) => `${context.className}.${context.methodName} (${context.relative}:${context.line})`)).join("; ") : "—";
    const purpose = used ? unique(contexts.map((context) => context.purpose)).join("; ") : "No active Angular match found";
    const pages = used ? unique(contexts.flatMap((context) => usageResolver.pagesFor(context))).join("; ") || "No static route resolved" : "—";
    const components = used ? unique(contexts.flatMap((context) => usageResolver.consumersFor(context).map((consumer) => consumer.className))).join("; ") || "No component resolved" : "—";
    return `| ${used ? "Used-In-Angular" : "Not-Used-In-Angular"} | ${markdownEscape(`${endpoint.verb} /api/${endpoint.route}`)} | ${markdownEscape(`${endpoint.className}.${endpoint.action}`)} | ${markdownEscape(functions)} | ${markdownEscape(purpose)} | ${markdownEscape(pages)} | ${markdownEscape(components)} |`;
  });
  return [
    "# Angular UI ↔ API Controller Usage Documentation",
    "",
    `Generated: ${new Date().toISOString()}`,
    "",
    "## Scope and matching rule",
    "",
    `- Angular source: \`${frontendRoot}\``,
    "- Backend source: `axionpro.api/Controllers`.",
    "- An endpoint is marked **Used-In-Angular** only when HTTP verb and normalized route both match an active Angular HTTP call.",
    "- **Not-Used-In-Angular** means no such active Angular call was found.",
    commentUnused
      ? "- Every Not-Used-In-Angular action in this report is commented in its controller inside `#region Unused`; uncommenting that region restores it."
      : "- Not-Used-In-Angular endpoints remain available in the backend until a `--comment-unused` run is requested.",
    "- UI pages are resolved statically from Angular route declarations. When that is not possible, the controller comment states this explicitly instead of guessing.",
    "",
    "## Summary",
    "",
    "| Measure | Total |",
    "|---|---:|",
    `| Backend controller endpoints scanned | ${endpoints.length} |`,
    `| Angular HTTP call-sites matched to backend endpoints | ${matched.length} |`,
    `| Backend endpoints marked Not-Used-In-Angular | ${unmatched} |`,
    `| Unused endpoint source state | ${commentUnused ? "Commented in #region Unused" : "Active"} |`,
    `| Angular call expressions parsed (before backend matching) | ${angularCalls.length} |`,
    `| Static routed Angular components resolved | ${componentRoutes.size} |`,
    "",
    "## Controller documentation convention",
    "",
    "Every controller action now has endpoint-level XML documentation. Used endpoints include Angular function, inferred UI purpose, resolved UI pages, and consuming component(s). Unused endpoints carry the exact `Not-Used-In-Angular` status.",
    commentUnused ? "Unused action source is line-commented inside a local `#region Unused`, so it can be restored without reconstructing code." : "",
    "",
    "## Complete endpoint matrix",
    "",
    "| Angular status | Backend endpoint | Controller action | Angular function/source | Angular purpose | Integrated UI page(s) | UI component(s) |",
    "|---|---|---|---|---|---|---|",
    ...rows,
    "",
  ].join("\n");
}

const angularRecords = await readAngularFiles();
const angularCalls = angularRecords.flatMap(extractAngularCalls);
const endpoints = await readBackendEndpoints();
const angularByKey = new Map();
for (const call of angularCalls) {
  const key = routeKey(call.verb, call.route);
  if (!angularByKey.has(key)) angularByKey.set(key, []);
  angularByKey.get(key).push(call);
}
const componentRoutes = await buildComponentRoutes();
const usageResolver = buildUsageResolver(angularRecords, componentRoutes);

const documentationUpdatesByFile = new Map();
if (applyDocs) {
  for (const endpoint of endpoints) {
    const attributeBlockStart = findAttributeBlockStart(endpoint.raw, endpoint.attributeIndex);
    const existingDoc = findXmlDocBlock(endpoint.raw, attributeBlockStart);
    const contexts = angularByKey.get(routeKey(endpoint.verb, endpoint.route)) ?? [];
    const start = existingDoc?.start ?? attributeBlockStart;
    const replacement = contexts.length ? buildMatchedDoc(endpoint, contexts, usageResolver) : buildNotUsedDoc(endpoint);
    if (!documentationUpdatesByFile.has(endpoint.file)) documentationUpdatesByFile.set(endpoint.file, []);
    documentationUpdatesByFile.get(endpoint.file).push({ start, end: attributeBlockStart, replacement: `${replacement}${endpoint.raw.includes("\r\n") ? "\r\n" : "\n"}` });
  }
}

const unusedEndpoints = endpoints.filter((endpoint) => !(angularByKey.get(routeKey(endpoint.verb, endpoint.route)) ?? []).length);
const unusedByAction = new Map();
for (const endpoint of endpoints) {
  const key = `${endpoint.file}|${endpoint.actionIndex}`;
  if (!unusedByAction.has(key)) unusedByAction.set(key, []);
  unusedByAction.get(key).push(endpoint);
}
const unusedCommentUpdatesByFile = new Map();
let fullyCommentedActions = 0;
let selectivelyCommentedRoutes = 0;
if (commentUnused) {
  for (const actionEndpoints of unusedByAction.values()) {
    const unusedInAction = actionEndpoints.filter((endpoint) => unusedEndpoints.includes(endpoint));
    if (!unusedInAction.length) continue;
    const file = actionEndpoints[0].file;
    if (!unusedCommentUpdatesByFile.has(file)) unusedCommentUpdatesByFile.set(file, []);
    if (unusedInAction.length === actionEndpoints.length) {
      unusedCommentUpdatesByFile.get(file).push(commentOutAction(actionEndpoints[0]));
      fullyCommentedActions++;
    } else {
      for (const endpoint of unusedInAction) {
        unusedCommentUpdatesByFile.get(file).push(commentOutHttpAttribute(endpoint));
        selectivelyCommentedRoutes++;
      }
    }
  }
}

const report = buildReport(endpoints, angularByKey, usageResolver, componentRoutes, angularCalls);
if (applyChanges) {
  if (applyDocs) {
    for (const [file, updates] of documentationUpdatesByFile) {
      let content = await fs.readFile(file, "utf8");
      for (const update of updates.sort((left, right) => right.start - left.start)) content = content.slice(0, update.start) + update.replacement + content.slice(update.end);
      await fs.writeFile(file, content, "utf8");
    }
  }
  if (commentUnused) {
    for (const [file, updates] of unusedCommentUpdatesByFile) {
      let content = await fs.readFile(file, "utf8");
      for (const update of updates.sort((left, right) => right.start - left.start)) content = content.slice(0, update.start) + update.replacement + content.slice(update.end);
      await fs.writeFile(file, content, "utf8");
    }
  }
  await fs.writeFile(reportPath, report, "utf8");
}

const matched = endpoints.filter((endpoint) => angularByKey.has(routeKey(endpoint.verb, endpoint.route)));
console.log(JSON.stringify({
  mode: commentUnused ? (applyChanges ? "comment-unused" : "comment-unused-dry-run") : applyDocs ? "apply" : "dry-run",
  backendEndpoints: endpoints.length,
  angularHttpCallSites: angularCalls.length,
  matchedEndpoints: matched.length,
  notUsedInAngular: endpoints.length - matched.length,
  routedAngularComponents: componentRoutes.size,
  matchedWithoutResolvedPage: matched.filter((endpoint) => !(angularByKey.get(routeKey(endpoint.verb, endpoint.route)) ?? []).some((context) => usageResolver.pagesFor(context).length)).length,
  fullyCommentedActions,
  selectivelyCommentedRoutes,
  report: relativeTo(workspace, reportPath),
}, null, 2));
